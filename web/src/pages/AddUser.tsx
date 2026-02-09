import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { createUser } from '../api/users';
import type { UserInput } from '../types/user';

export default function AddUser() {
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [form, setForm] = useState<UserInput>({
    name: '',
    username: '',
    email: '',
  });

  const update = (field: keyof UserInput, value: string | undefined) => {
    setForm((prev) => ({ ...prev, [field]: value }));
    setError(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      await createUser({
        name: form.name.trim(),
        username: form.username.trim(),
        email: form.email.trim(),
      });
      navigate('/', { replace: true });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Something went wrong');
    } finally {
      setSubmitting(false);
    }
  };

  const inputClass =
    'w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-800 focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500';

  return (
    <div className='min-h-screen bg-slate-50'>
      <div className='mx-auto max-w-2xl px-4 py-6'>
        <header className='mb-6 flex items-center justify-between gap-4'>
          <h1 className='m-0 text-2xl font-semibold text-slate-800'>Add user</h1>
          <Link
            to='/'
            className='inline-block rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-100'
          >
            ← Back to list
          </Link>
        </header>

        <main className='rounded-lg border border-slate-200 bg-white p-6 shadow-sm'>
          <form onSubmit={handleSubmit} className='flex flex-col gap-4'>
            {error && (
              <p className='rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-800' role='alert'>
                {error}
              </p>
            )}
            <div>
              <label htmlFor='name' className='mb-1 block text-sm font-medium text-slate-700'>
                Name *
              </label>
              <input
                id='name'
                type='text'
                required
                value={form.name}
                onChange={(e) => update('name', e.target.value)}
                placeholder='Jane Doe'
                className={inputClass}
              />
            </div>
            <div>
              <label htmlFor='username' className='mb-1 block text-sm font-medium text-slate-700'>
                Username *
              </label>
              <input
                id='username'
                type='text'
                required
                value={form.username}
                onChange={(e) => update('username', e.target.value)}
                placeholder='janedoe'
                className={inputClass}
              />
            </div>
            <div>
              <label htmlFor='email' className='mb-1 block text-sm font-medium text-slate-700'>
                Email *
              </label>
              <input
                id='email'
                type='email'
                required
                value={form.email}
                onChange={(e) => update('email', e.target.value)}
                placeholder='jane@example.com'
                className={inputClass}
              />
            </div>
            <div className='mt-2 flex gap-3'>
              <button
                type='submit'
                disabled={submitting}
                className='rounded-lg bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700 disabled:opacity-50 disabled:pointer-events-none'
              >
                {submitting ? 'Adding…' : 'Add user'}
              </button>
              <Link
                to='/'
                className='rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-100'
              >
                Cancel
              </Link>
            </div>
          </form>
        </main>
      </div>
    </div>
  );
}
