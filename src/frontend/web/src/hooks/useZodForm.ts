import { zodResolver } from '@hookform/resolvers/zod';

/**
 * Wraps zodResolver with the necessary type cast.
 * Zod's input types (before transforms/defaults) differ from output types,
 * causing a mismatch with react-hook-form's Resolver type signature.
 */
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function typedZodResolver(schema: any): any {
  return zodResolver(schema);
}
