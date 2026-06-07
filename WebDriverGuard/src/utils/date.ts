import { format, parseISO } from 'date-fns';
import { uk, enUS } from 'date-fns/locale';

export function formatDate(dateStr: string, lang: string): string {
  try {
    const date = parseISO(dateStr);
    const locale = lang === 'uk' ? uk : enUS;
    const pattern = lang === 'uk' ? 'dd.MM.yyyy HH:mm' : 'MM/dd/yyyy hh:mm a';
    return format(date, pattern, { locale });
  } catch {
    return dateStr;
  }
}
