import { View, Text, StyleSheet } from 'react-native';
import { Colors, Spacing, FontSize } from '../../styles/tokens';

type DetailRowProps = {
  label: string;
  value?: string | number | null;
};

export function DetailRow({ label, value }: DetailRowProps) {
  if (value == null || value === '') return null;
  return (
    <View style={styles.row}>
      <Text style={styles.label}>{label}</Text>
      <Text style={styles.value}>{String(value)}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  row: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingVertical: Spacing.xs,
  },
  label: {
    fontSize: FontSize.body,
    color: Colors.textTertiary,
  },
  value: {
    fontSize: FontSize.body,
    fontWeight: '500',
    color: Colors.textPrimary,
    flex: 1,
    textAlign: 'right',
    marginLeft: Spacing.lg,
  },
});
