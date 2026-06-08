"""
Simple round-robin reverse proxy for local scaling demo.
Usage:
  pip install aiohttp
  python proxy.py --backends http://localhost:8080 http://localhost:8081 http://localhost:8082
  # proxy listens on http://localhost:9000
"""

import argparse
import asyncio
import itertools
import aiohttp
from aiohttp import web

parser = argparse.ArgumentParser()
parser.add_argument("--backends", nargs="+",
    default=["http://localhost:8080", "http://localhost:8081"],
    help="List of backend URLs")
parser.add_argument("--port", type=int, default=9000)
args = parser.parse_args()

backends = itertools.cycle(args.backends)
print(f"[proxy] Backends: {args.backends}")
print(f"[proxy] Listening on http://localhost:{args.port}")

async def handle(request: web.Request):
    backend = next(backends)
    url = backend + str(request.rel_url)
    headers = {k: v for k, v in request.headers.items()
               if k.lower() not in ("host", "content-length")}
    body = await request.read()
    try:
        async with aiohttp.ClientSession() as session:
            async with session.request(
                method=request.method,
                url=url,
                headers=headers,
                data=body if body else None,
                allow_redirects=False,
                timeout=aiohttp.ClientTimeout(total=15),
            ) as resp:
                resp_body = await resp.read()
                return web.Response(
                    status=resp.status,
                    body=resp_body,
                    headers={k: v for k, v in resp.headers.items()
                             if k.lower() not in ("transfer-encoding", "content-encoding")},
                )
    except Exception as e:
        return web.Response(status=502, text=f"Proxy error -> {backend}: {e}")

app = web.Application()
app.router.add_route("*", "/{path_info:.*}", handle)

if __name__ == "__main__":
    web.run_app(app, port=args.port, access_log=None)
