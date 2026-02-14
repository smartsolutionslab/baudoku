import { useState, useEffect, useRef, useCallback } from "react";
import * as installationRepo from "../db/repositories/installationRepo";
import type { SearchResult } from "../db/repositories/installationRepo";
import type { ProjectId } from "../types/branded";

type SearchFilters = {
  status?: string[];
  projectId?: ProjectId;
};

type UseInstallationSearchReturn = {
  query: string;
  setQuery: (q: string) => void;
  filters: SearchFilters;
  setFilters: (f: SearchFilters) => void;
  toggleStatus: (status: string) => void;
  results: SearchResult[];
  searching: boolean;
};

const DEBOUNCE_MS = 300;

export function useInstallationSearch(): UseInstallationSearchReturn {
  const [query, setQuery] = useState("");
  const [filters, setFilters] = useState<SearchFilters>({});
  const [results, setResults] = useState<SearchResult[]>([]);
  const [searching, setSearching] = useState(false);
  const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const doSearch = useCallback(
    async (q: string, f: SearchFilters) => {
      setSearching(true);
      const data = await installationRepo.search(q, f);
      setResults(data);
      setSearching(false);
    },
    []
  );

  useEffect(() => {
    if (timerRef.current) clearTimeout(timerRef.current);
    timerRef.current = setTimeout(() => {
      void doSearch(query, filters);
    }, DEBOUNCE_MS);

    return () => {
      if (timerRef.current) clearTimeout(timerRef.current);
    };
  }, [query, filters, doSearch]);

  const toggleStatus = useCallback((status: string) => {
    setFilters((prev) => {
      const current = prev.status ?? [];
      const next = current.includes(status)
        ? current.filter((s) => s !== status)
        : [...current, status];
      return { ...prev, status: next };
    });
  }, []);

  return { query, setQuery, filters, setFilters, toggleStatus, results, searching };
}
