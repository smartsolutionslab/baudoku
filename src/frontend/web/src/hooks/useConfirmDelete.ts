import { useState, useCallback } from 'react';

type UseConfirmDeleteReturn<T> = {
  deleteId: T | null;
  requestDelete: (id: T) => void;
  confirmProps: {
    open: boolean;
    onConfirm: () => void;
    onCancel: () => void;
  };
};

export function useConfirmDelete<T>(onConfirm: (id: T) => void): UseConfirmDeleteReturn<T> {
  const [deleteId, setDeleteId] = useState<T | null>(null);

  const requestDelete = useCallback((id: T) => {
    setDeleteId(id);
  }, []);

  const confirmProps = {
    open: deleteId !== null,
    onConfirm: () => {
      if (deleteId !== null) {
        onConfirm(deleteId);
        setDeleteId(null);
      }
    },
    onCancel: () => {
      setDeleteId(null);
    },
  };

  return { deleteId, requestDelete, confirmProps };
}
