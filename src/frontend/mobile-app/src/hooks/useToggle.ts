import { useState, useCallback } from 'react';

type UseToggleReturn = {
  value: boolean;
  open: () => void;
  close: () => void;
  toggle: () => void;
};

export function useToggle(initial = false): UseToggleReturn {
  const [value, setValue] = useState(initial);
  const open = useCallback(() => setValue(true), []);
  const close = useCallback(() => setValue(false), []);
  const toggle = useCallback(() => setValue((v) => !v), []);
  return { value, open, close, toggle };
}
