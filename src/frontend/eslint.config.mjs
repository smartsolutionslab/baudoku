import js from '@eslint/js';
import tseslint from 'typescript-eslint';
import react from 'eslint-plugin-react';
import reactHooks from 'eslint-plugin-react-hooks';
import prettier from 'eslint-config-prettier';
import globals from 'globals';

export default tseslint.config(
  // 1. Global ignores
  {
    ignores: [
      'node_modules/',
      '**/dist/',
      '.expo/',
      'android/',
      'ios/',
      'coverage/',
      'drizzle/meta/',
      'drizzle/migrations.js',
      'expo-env.d.ts',
      '*.tsbuildinfo',
      // CJS config files
      '**/babel.config.js',
      '**/metro.config.js',
      '**/jest.config.js',
    ],
  },

  // 2. Core JS recommended
  js.configs.recommended,

  // 3. TypeScript strict (non-type-checked — fast)
  ...tseslint.configs.strict,

  // 4. Custom TS overrides
  {
    files: ['**/*.{ts,tsx}'],
    rules: {
      '@typescript-eslint/consistent-type-definitions': ['error', 'type'],
      '@typescript-eslint/consistent-type-imports': [
        'error',
        { prefer: 'type-imports', fixStyle: 'inline-type-imports' },
      ],
      '@typescript-eslint/no-unused-vars': [
        'error',
        { argsIgnorePattern: '^_', varsIgnorePattern: '^_' },
      ],
      '@typescript-eslint/no-explicit-any': 'warn',
      '@typescript-eslint/no-non-null-assertion': 'warn',
    },
  },

  // 5. React rules (mobile-app + web only)
  {
    files: ['mobile-app/**/*.{ts,tsx}', 'web/**/*.{ts,tsx}'],
    plugins: {
      react,
      'react-hooks': reactHooks,
    },
    languageOptions: {
      globals: {
        ...globals.browser,
      },
    },
    settings: {
      react: { version: 'detect' },
    },
    rules: {
      ...react.configs.recommended.rules,
      ...reactHooks.configs.recommended.rules,
      'react/react-in-jsx-scope': 'off',
      'react/prop-types': 'off',
    },
  },

  // 6. CommonJS entry point
  {
    files: ['mobile-app/index.js'],
    languageOptions: {
      globals: {
        ...globals.node,
      },
    },
  },

  // 7. Prettier — disables formatting rules (must be last)
  prettier,
);
