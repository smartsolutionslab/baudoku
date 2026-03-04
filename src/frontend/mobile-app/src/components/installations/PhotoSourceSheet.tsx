import { Text, TouchableOpacity, StyleSheet } from 'react-native';
import { FontAwesome } from '@expo/vector-icons';
import { OptionSheet, optionSheetStyles } from '../common';
import { Colors } from '../../styles/tokens';

type PhotoSourceSheetProps = {
  visible: boolean;
  onCamera: () => void;
  onGallery: () => void;
  onClose: () => void;
};

export function PhotoSourceSheet({ visible, onCamera, onGallery, onClose }: PhotoSourceSheetProps) {
  return (
    <OptionSheet visible={visible} onClose={onClose} title="Foto hinzufügen">
      <TouchableOpacity style={styles.option} onPress={onCamera}>
        <FontAwesome name="camera" size={20} color={Colors.primary} style={styles.icon} />
        <Text style={optionSheetStyles.optionText}>Foto aufnehmen</Text>
      </TouchableOpacity>

      <TouchableOpacity style={styles.option} onPress={onGallery}>
        <FontAwesome name="image" size={20} color={Colors.primary} style={styles.icon} />
        <Text style={optionSheetStyles.optionText}>Aus Galerie wählen</Text>
      </TouchableOpacity>
    </OptionSheet>
  );
}

const styles = StyleSheet.create({
  option: {
    ...optionSheetStyles.option,
    flexDirection: 'row',
    alignItems: 'center',
  },
  icon: {
    width: 28,
  },
});
