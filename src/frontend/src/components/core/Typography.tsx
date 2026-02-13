import React from "react";
import { Text, StyleSheet, type TextStyle } from "react-native";
import { Colors, FontSize } from "../../styles/tokens";

type TypographyProps = {
  children: React.ReactNode;
  style?: TextStyle;
  numberOfLines?: number;
};

export function Title({ children, style, numberOfLines }: TypographyProps) {
  return (
    <Text style={[styles.title, style]} numberOfLines={numberOfLines}>
      {children}
    </Text>
  );
}

export function Headline({ children, style, numberOfLines }: TypographyProps) {
  return (
    <Text style={[styles.headline, style]} numberOfLines={numberOfLines}>
      {children}
    </Text>
  );
}

export function Body({ children, style, numberOfLines }: TypographyProps) {
  return (
    <Text style={[styles.body, style]} numberOfLines={numberOfLines}>
      {children}
    </Text>
  );
}

export function Caption({ children, style, numberOfLines }: TypographyProps) {
  return (
    <Text style={[styles.caption, style]} numberOfLines={numberOfLines}>
      {children}
    </Text>
  );
}

const styles = StyleSheet.create({
  title: {
    fontSize: FontSize.title,
    fontWeight: "700",
    color: Colors.textPrimary,
  },
  headline: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    color: Colors.textPrimary,
  },
  body: {
    fontSize: FontSize.body,
    fontWeight: "400",
    color: Colors.textPrimary,
  },
  caption: {
    fontSize: FontSize.caption,
    fontWeight: "400",
    color: Colors.textTertiary,
  },
});
