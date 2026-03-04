import { Text, TouchableOpacity } from 'react-native';
import { PHOTO_TYPES, PHOTO_TYPE_LABELS } from '@baudoku/documentation';
import { OptionSheet, optionSheetStyles } from '../common';

export type PhotoType = (typeof PHOTO_TYPES)[number];

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
          key={pt}
          style={optionSheetStyles.option}
          onPress={() => onSelect(pt)}
        >
          <Text style={optionSheetStyles.optionText}>{PHOTO_TYPE_LABELS[pt]}</Text>
        </TouchableOpacity>
      ))}
    </OptionSheet>
  );
}
