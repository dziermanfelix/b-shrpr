import type { User, UserInput } from '../types/user'

const API = '/api'

export async function getUsers(): Promise<User[]> {
  const res = await fetch(`${API}/users`)
  if (!res.ok) throw new Error('Failed to load users')
  return res.json()
}

export async function createUser(input: UserInput): Promise<User> {
  const res = await fetch(`${API}/users`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(input),
  })
  if (!res.ok) {
    const err = await res.text()
    throw new Error(err || 'Failed to create user')
  }
  return res.json()
}
