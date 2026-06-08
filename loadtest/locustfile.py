"""
DriverGuard Load Test — Locust
Usage:
  locust -f locustfile.py --host http://localhost:8081
  # headless:
  locust -f locustfile.py --host http://localhost:8081 \
         --headless -u 100 -r 10 -t 60s --csv=results/run1
"""

import random
import string
from locust import HttpUser, task, between, events

# ---------------------------------------------------------------------------
# Shared token cache so each virtual user logs in once and reuses the token
# ---------------------------------------------------------------------------

TEST_EMAIL_PREFIX = "loadtest_"
TEST_PASSWORD     = "LoadTest123!"


def rand_email() -> str:
    suffix = "".join(random.choices(string.ascii_lowercase, k=8))
    return f"{TEST_EMAIL_PREFIX}{suffix}@test.local"


# ---------------------------------------------------------------------------
# Scenario 1 — unauthenticated health check (pure throughput baseline)
# ---------------------------------------------------------------------------
class HealthCheckUser(HttpUser):
    """
    Hits GET /api/health only.
    No auth, minimal server work — good for measuring raw throughput ceiling.
    """
    weight       = 2
    wait_time    = between(0.05, 0.2)   # aggressive, ~5-20 RPS per user

    @task
    def health(self):
        self.client.get("/api/health", name="GET /api/health")


# ---------------------------------------------------------------------------
# Scenario 2 — full user journey (register → login → list devices)
# ---------------------------------------------------------------------------
class AuthenticatedUser(HttpUser):
    """
    Simulates a real user: registers (once), logs in (once), then repeatedly
    fetches their device list and event list.
    """
    weight    = 5
    wait_time = between(0.5, 2.0)

    def on_start(self):
        """Called once per virtual user at startup — register + login."""
        self.token   = None
        self.email   = rand_email()
        self.headers = {}

        # Try to register (may return 400 if email already exists — that's OK)
        self.client.post(
            "/api/auth/register",
            json={"email": self.email, "password": TEST_PASSWORD},
            name="POST /api/auth/register",
        )

        # Login
        with self.client.post(
            "/api/auth/login",
            json={"email": self.email, "password": TEST_PASSWORD},
            name="POST /api/auth/login",
            catch_response=True,
        ) as resp:
            if resp.status_code == 200:
                self.token   = resp.json().get("token", "")
                self.headers = {"Authorization": f"Bearer {self.token}"}
            else:
                resp.failure(f"Login failed: {resp.status_code} {resp.text[:120]}")

    @task(5)
    def list_my_devices(self):
        """Most frequent: list own devices — hits DB."""
        if not self.token:
            return
        self.client.get(
            "/api/devices/my",
            headers=self.headers,
            name="GET /api/devices/my",
        )

    @task(2)
    def health(self):
        """Mix in some health checks."""
        self.client.get("/api/health", name="GET /api/health")

    @task(1)
    def get_notifications(self):
        """Notification list — less frequent."""
        if not self.token:
            return
        self.client.get(
            "/api/notifications",
            headers=self.headers,
            name="GET /api/notifications",
        )
