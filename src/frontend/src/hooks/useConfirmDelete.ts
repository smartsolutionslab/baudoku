import { useState, useCallback } from "react";
import { Alert } from "react-native";

interface UseConfirmDeleteReturn {
  deleting: boolean;
  confirmDelete: (opts: {
    title: string;
    message: string;
    onConfirm: () => Promise<void>;
  }) => void;
}

export function useConfirmDelete(): UseConfirmDeleteReturn {
  const [deleting, setDeleting] = useState(false);

  const confirmDelete = useCallback(
    ({
      title,
      message,
      onConfirm,
    }: {
      title: string;
      message: string;
      onConfirm: () => Promise<void>;
    }) => {
      Alert.alert(title, message, [
        { text: "Abbrechen", style: "cancel" },
        {
          text: "Löschen",
          style: "destructive",
          onPress: async () => {
            setDeleting(true);
            try {
              await onConfirm();
            } finally {
              setDeleting(false);
            }
          },
        },
      ]);
    },
    []
  );

  return { deleting, confirmDelete };
}
