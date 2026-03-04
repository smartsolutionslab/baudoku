import type { UseFormRegister } from 'react-hook-form';

type NotesProps = {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  register: UseFormRegister<any>;
};

export function InstallationNotesSection({ register }: NotesProps) {
  return (
    <div className="rounded-xl border border-gray-200 bg-white p-6">
      <h2 className="text-base font-semibold text-gray-900">Notizen</h2>
      <div className="mt-4">
        <textarea
          {...register('notes')}
          rows={3}
          className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
          placeholder="Zusätzliche Bemerkungen..."
        />
      </div>
    </div>
  );
}
