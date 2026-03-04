export function formatAddress(
  street?: string | null,
  zipCode?: string | null,
  city?: string | null,
): string {
  return [street, zipCode, city].filter(Boolean).join(', ');
}
