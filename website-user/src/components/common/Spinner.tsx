import { LoaderCircle } from 'lucide-react';

export function Spinner() {
  return (
    <div className="grid min-h-48 place-items-center">
      <LoaderCircle className="animate-spin text-coral" size={30} />
    </div>
  );
}
