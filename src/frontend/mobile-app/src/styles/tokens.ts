export const Colors = {
  primary: "#007AFF",
  success: "#34C759",
  warning: "#FF9500",
  danger: "#FF3B30",
  background: "#F2F2F7",
  card: "#FFFFFF",
  white: "#FFFFFF",
  black: "#000000",
  textPrimary: "#000000",
  textSecondary: "#3C3C43",
  textTertiary: "#8E8E93",
  separator: "#C6C6C8",
  disabled: "#C7C7CC",
  overlay: "rgba(0,0,0,0.4)",
  overlayLight: "rgba(0,0,0,0.3)",
  primaryLight: "rgba(0, 122, 255, 0.1)",
  optionActiveBg: "#E8F0FE",
  warningText: "#856404",
  errorBg: "#FFF0F0",
  textQuaternary: "#AAAAAA",
  // Zone types
  zoneBuilding: "#5856D6",
  zoneFloor: "#AF52DE",
  // GPS quality backgrounds
  gpsGradeB: "#A8D500",
  gpsBgA: "#E8F5E9",
  gpsBgB: "#F1F8E9",
  gpsBgC: "#FFF8E1",
  gpsBgD: "#FFEBEE",
  // Diff highlight
  diffHighlight: "#FFF3CD",
};

export const Spacing = { xs: 4, sm: 8, md: 12, lg: 16, xl: 24 };

export const FontSize = {
  footnote: 12,
  caption: 13,
  body: 15,
  callout: 16,
  headline: 17,
  title: 24,
};

export const FontFamily = {
  mono: "SpaceMono",
};

export const Radius = { xs: 4, sm: 8, md: 10, lg: 12, xl: 16 };

export const Shadows = {
  card: {
    shadowColor: Colors.black,
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.1,
    shadowRadius: 3,
    elevation: 2,
  },
  elevated: {
    shadowColor: Colors.black,
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.25,
    shadowRadius: 4,
    elevation: 5,
  },
  floating: {
    shadowColor: Colors.black,
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.15,
    shadowRadius: 6,
    elevation: 4,
  },
} as const;
