type LoadingSkeletonProps = {
  count?: number;
  itemClassName?: string;
  layout?: 'grid' | 'list';
};

export function LoadingSkeleton({
  count = 6,
  itemClassName = 'h-32',
  layout = 'grid',
}: LoadingSkeletonProps) {
  return (
    <div>
      <div className="h-8 w-48 animate-pulse rounded bg-gray-200" />
      <div
        className={
          layout === 'grid' ? 'mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3' : 'mt-6 space-y-3'
        }
      >
        {Array.from({ length: count }).map((_, i) => (
          <div
            key={i}
            className={`${itemClassName} animate-pulse rounded-xl border border-gray-200 bg-gray-100`}
          />
        ))}
      </div>
    </div>
  );
}
