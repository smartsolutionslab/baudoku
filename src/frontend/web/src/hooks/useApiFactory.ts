import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiGet, apiPost, apiPut, apiDelete } from '@baudoku/core';

export function useApiQuery<TData>(
  queryKey: readonly unknown[],
  url: string,
  enabled?: boolean,
) {
  return useQuery({
    queryKey,
    queryFn: () => apiGet<TData>(url),
    enabled,
  });
}

export function useApiMutation<TVariables, TData = unknown>(opts: {
  mutationFn: (variables: TVariables) => Promise<TData>;
  invalidateKeys: readonly (readonly unknown[])[];
}) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: opts.mutationFn,
    onSuccess: () => {
      for (const key of opts.invalidateKeys) {
        queryClient.invalidateQueries({ queryKey: [...key] });
      }
    },
  });
}

export function useApiPost<TData, TBody = unknown>(
  url: string,
  invalidateKeys: readonly (readonly unknown[])[],
) {
  return useApiMutation<TBody, TData>({
    mutationFn: (data) => apiPost<TData>(url, data),
    invalidateKeys,
  });
}

export function useApiPut<TData, TBody = unknown>(
  url: string,
  invalidateKeys: readonly (readonly unknown[])[],
) {
  return useApiMutation<TBody, TData>({
    mutationFn: (data) => apiPut<TData>(url, data),
    invalidateKeys,
  });
}

export function useApiDelete(
  urlFn: (id: string) => string,
  invalidateKeys: readonly (readonly unknown[])[],
) {
  return useApiMutation<string>({
    mutationFn: (id) => apiDelete(urlFn(id)),
    invalidateKeys,
  });
}
