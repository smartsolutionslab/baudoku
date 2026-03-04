import React from 'react';
import { QueryClient, QueryClientProvider, MutationCache, QueryCache } from '@tanstack/react-query';
import { useToastStore } from '../store';
import { ApiError } from '@baudoku/core';
import { API_ERRORS } from '../constants/strings';

function getErrorMessage(error: unknown): string | null {
  if (error instanceof ApiError) {
    if (error.status === 401) return null;
    if (error.status === 409) return API_ERRORS.conflict;
    if (error.status === 422) return API_ERRORS.validation;
    if (error.status >= 500) return API_ERRORS.server;
  }
  if (error instanceof Error) return error.message;
  return API_ERRORS.unknown;
}

const mutationCache = new MutationCache({
  onError: (error, _variables, _context, mutation) => {
    const detail = getErrorMessage(error);
    if (detail === null) return;

    const contextMessage = (mutation.meta as Record<string, unknown>)?.errorMessage as
      | string
      | undefined;

    const message = contextMessage ? `${contextMessage}: ${detail}` : detail;
    useToastStore.getState().show(message, 'error');
  },
});

const queryCache = new QueryCache({
  onError: (error) => {
    const detail = getErrorMessage(error);
    if (detail === null) return;
    useToastStore.getState().show(detail, 'error');
  },
});

const queryClient = new QueryClient({
  queryCache,
  mutationCache,
  defaultOptions: {
    queries: {
      staleTime: Infinity,
      gcTime: Infinity,
      retry: false,
      refetchOnWindowFocus: false,
      refetchOnReconnect: false,
    },
  },
});

export function QueryProvider({ children }: { children: React.ReactNode }) {
  return <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>;
}

export { queryClient };
