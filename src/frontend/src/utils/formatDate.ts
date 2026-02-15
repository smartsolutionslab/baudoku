export function formatDate(d: Date | null | undefined): string {
  if (!d) return "";
  return d.toLocaleDateString("de-DE", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });
}
