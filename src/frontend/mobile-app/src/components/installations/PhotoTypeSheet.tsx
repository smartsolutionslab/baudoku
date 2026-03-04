import { Text, TouchableOpacity } from 'react-native';
import { OptionSheet, optionSheetStyles } from '../common/OptionSheet';

const PHOTO_TYPES = [
  { value: 'before', label: 'Vorher' },
  { value: 'after', label: 'Nachher' },
  { value: 'detail', label: 'Detail' },
  { value: 'overview', label: 'Übersicht' },
] as const;

export type PhotoType = (typeof PHOTO_TYPES)[number]['value'];

type PhotoTypeSheetProps = {
  visible: boolean;
  onSelect: (type: PhotoType) => void;
  onClose: () => void;
};

export function PhotoTypeSheet({ visible, onSelect, onClose }: PhotoTypeSheetProps) {
  return (
    <OptionSheet visible={visible} onClose={onClose} title="Foto-Typ wählen">
      {PHOTO_TYPES.map((pt) => (
        <TouchableOpacity
          key={pt.value}
          style={optionSheetStyles.option}
          onPress={() => onSelect(pt.value)}
        >
          <Text style={optionSheetStyles.optionText}>{pt.label}</Text>
        </TouchableOpacity>
      ))}
    </OptionSheet>
  );
}
