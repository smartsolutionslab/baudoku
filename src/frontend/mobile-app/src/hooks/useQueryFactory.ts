import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';

export function useListQuery<TData>(
  queryKey: readonly unknown[],
  queryFn: () => Promise<TData>,
  enabled?: boolean,
) {
  return useQuery({ queryKey, queryFn, enabled });
}

export function useSyncMutation<TVariables, TData = unknown>(opts: {
  mutationFn: (variables: TVariables) => Promise<TData>;
  errorMessage: string;
  invalidateKeys: readonly (readonly unknown[])[];
  onSuccessKeys?: (variables: TVariables) => readonly (readonly unknown[])[];
}) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: opts.mutationFn,
    meta: { errorMessage: opts.errorMessage },
    onSuccess: (_data: TData, variables: TVariables) => {
      const keys = opts.onSuccessKeys ? opts.onSuccessKeys(variables) : opts.invalidateKeys;
      for (const key of keys) {
        queryClient.invalidateQueries({ queryKey: [...key] });
      }
      queryClient.invalidateQueries({ queryKey: ['syncStatus'] });
    },
  });
}
