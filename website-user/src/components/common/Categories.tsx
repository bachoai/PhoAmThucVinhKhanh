import { useQuery } from '@tanstack/react-query';
import { categoryApi } from '../../api/categoryApi';
import { getCopy } from '../../i18n/copy';
import { useAppStore } from '../../store/appStore';

export function Categories({
  selected,
  onSelect,
}: {
  selected?: string;
  onSelect?: (id?: string) => void;
}) {
  const lang = useAppStore((state) => state.lang);
  const ui = getCopy(lang);
  const { data = [] } = useQuery({
    queryKey: ['categories'],
    queryFn: categoryApi.list,
  });

  return (
    <div className="flex gap-2 overflow-x-auto pb-2">
      <button
        type="button"
        onClick={() => onSelect?.()}
        className={`pill whitespace-nowrap ${!selected ? 'border-coral bg-orange-50 text-coral dark:bg-orange-500/15' : ''}`}
      >
        {ui.common.all}
      </button>

      {data.map((category) => (
        <button
          type="button"
          key={category.id}
          onClick={() => onSelect?.(category.id)}
          className={`pill whitespace-nowrap ${selected === category.id ? 'border-coral bg-orange-50 text-coral dark:bg-orange-500/15' : 'hover:border-teal'}`}
        >
          {category.name}
        </button>
      ))}
    </div>
  );
}
