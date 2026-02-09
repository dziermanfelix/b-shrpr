import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getUsers } from '../api/users'
import type { User } from '../types/user'

export default function Home() {
  const [users, setUsers] = useState<User[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = () => {
    setLoading(true)
    setError(null)
    getUsers()
      .then(setUsers)
      .catch(() => setError('Failed to load users'))
      .finally(() => setLoading(false))
  }

  useEffect(() => {
    load()
  }, [])

  return (
    <div className="min-h-screen bg-slate-50">
      <div className="mx-auto max-w-2xl px-4 py-6">
        <header className="mb-6 flex items-center justify-between gap-4">
          <h1 className="m-0 text-2xl font-semibold text-slate-800">
            Admin — Users
          </h1>
          <Link
            to="/add"
            className="inline-block rounded-lg bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700"
          >
            Add user
          </Link>
        </header>

        <main className="rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
          {error && (
            <p
              className="mb-4 rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-800"
              role="alert"
            >
              {error}
            </p>
          )}
          {loading ? (
            <p className="m-0 text-sm text-slate-500">Loading…</p>
          ) : users.length === 0 ? (
            <p className="m-0 text-sm text-slate-500">
              No users yet. Add one to get started.
            </p>
          ) : (
            <ul className="m-0 list-none p-0">
              {users.map((u) => (
                <li
                  key={u.id}
                  className="grid grid-cols-[1fr_auto_auto] items-center gap-4 border-b border-slate-200 py-3 last:border-b-0"
                >
                  <span className="font-medium text-slate-800">{u.name}</span>
                  <span className="text-sm text-slate-500">@{u.username}</span>
                  <span className="text-sm text-slate-500">{u.email}</span>
                </li>
              ))}
            </ul>
          )}
        </main>
      </div>
    </div>
  )
}
