import type { ReactNode } from 'react';
import { Text, TouchableOpacity, StyleSheet } from 'react-native';
import { BottomSheet } from './BottomSheet';
import { Colors, Spacing, FontSize, Radius } from '../../styles/tokens';

type OptionSheetProps = {
  visible: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
};

export function OptionSheet({ visible, onClose, title, children }: OptionSheetProps) {
  return (
    <BottomSheet visible={visible} onClose={onClose} title={title}>
      {children}
      <TouchableOpacity style={styles.cancelButton} onPress={onClose}>
        <Text style={styles.cancelText}>Abbrechen</Text>
      </TouchableOpacity>
    </BottomSheet>
  );
}

export const optionSheetStyles = StyleSheet.create({
  option: {
    paddingVertical: Spacing.md,
    borderBottomWidth: StyleSheet.hairlineWidth,
    borderBottomColor: Colors.separator,
  },
  optionText: {
    fontSize: FontSize.callout,
    color: Colors.textPrimary,
  },
});

const styles = StyleSheet.create({
  cancelButton: {
    alignItems: 'center',
    paddingVertical: 14,
    marginTop: Spacing.lg,
    backgroundColor: Colors.background,
    borderRadius: Radius.md,
  },
  cancelText: {
    fontSize: FontSize.callout,
    fontWeight: '600',
    color: Colors.textTertiary,
  },
});
