import { useParams, Link, useNavigate } from 'react-router-dom'
import { useMemo, useState } from 'react'
import { useProjectTickets } from '../api/queries'
import { useCreateTicket } from '../api/mutations'
import { CreateTicketInput } from '../api/schemas'

const statusOptions = ['','New','InProgress','Resolved','Closed'] as const
const priorityOptions = ['','Low','Medium','High'] as const

export default function ProjectTickets() {
  const { projectId = '' } = useParams()
  const navigate = useNavigate()
  const [status, setStatus] = useState<string>('')
  const [priority, setPriority] = useState<string>('')
  const [search, setSearch] = useState<string>('')
  const [page, setPage] = useState(1)
  const pageSize = 10

  const [showCreate, setShowCreate] = useState(false)
  const [title, setTitle] = useState('')
  const [desc, setDesc] = useState('')
  const [newPriority, setNewPriority] = useState<'Low'|'Medium'|'High'>('Medium')

  const filters = useMemo(() => ({ status, priority, search, page, pageSize }), [status, priority, search, page])

  const { data, isLoading, isError, error } = useProjectTickets(projectId, filters)
  const createMutation = useCreateTicket(projectId)

  async function handleCreate(e: React.FormEvent) {
    e.preventDefault()
    const payload: CreateTicketInput = {
      projectId,
      title: title.trim(),
      description: desc.trim() || undefined,
      priority: newPriority
    }
    if (payload.title.length < 5) return
    const created = await createMutation.mutateAsync(payload)
    // go to details
    navigate(`/tickets/${created.id}`)
  }

  return (
    <section>
      <h2>Tickets</h2>

      <button onClick={() => setShowCreate(v => !v)} style={{ margin: '8px 0' }}>
        {showCreate ? 'Cancel' : 'New Ticket'}
      </button>

      {showCreate && (
        <form onSubmit={handleCreate} style={{ border: '1px solid #ddd', borderRadius: 8, padding: 12, marginBottom: 12 }}>
          <div style={{ display: 'grid', gap: 8 }}>
            <input value={title} onChange={e => setTitle(e.target.value)} placeholder="Title (5–120 chars)" />
            <textarea value={desc} onChange={e => setDesc(e.target.value)} placeholder="Description (optional)" rows={3} />
            <label>
              Priority:&nbsp;
              <select value={newPriority} onChange={e => setNewPriority(e.target.value as typeof newPriority)}>
                {(['Low','Medium','High'] as const).map(p => <option key={p} value={p}>{p}</option>)}
              </select>
            </label>
          </div>
          <button type="submit" disabled={createMutation.isPending} style={{ marginTop: 8 }}>
            {createMutation.isPending ? 'Creating…' : 'Create'}
          </button>
        </form>
      )}

      <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap', margin: '8px 0 16px' }}>
        <select value={status} onChange={(e) => { setPage(1); setStatus(e.target.value) }}>
          {statusOptions.map(o => <option key={o} value={o}>{o || 'Any status'}</option>)}
        </select>
        <select value={priority} onChange={(e) => { setPage(1); setPriority(e.target.value) }}>
          {priorityOptions.map(o => <option key={o} value={o}>{o || 'Any priority'}</option>)}
        </select>
        <input
          placeholder="Search title/description"
          value={search}
          onChange={(e) => { setPage(1); setSearch(e.target.value) }}
          style={{ flex: 1, minWidth: 200 }}
        />
      </div>

      {isLoading && <p>Loading tickets…</p>}
      {isError && <p style={{ color: 'crimson' }}>Error: {error?.message}</p>}

      {data ? (
        <>
          <table style={{ width: '100%', borderCollapse: 'collapse' }}>
            <thead>
              <tr>
                <th style={th}>Title</th>
                <th style={th}>Status</th>
                <th style={th}>Priority</th>
                <th style={th}>Assignee</th>
                <th style={th}>Updated</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map(t => (
                <tr key={t.id} style={{ borderTop: '1px solid #ddd' }}>
                  <td style={td}><Link to={`/tickets/${t.id}`}>{t.title}</Link></td>
                  <td style={td}>{t.status}</td>
                  <td style={td}>{t.priority}</td>
                  <td style={td}>{t.assignee ?? '-'}</td>
                  <td style={td}>{new Date(t.updatedAt).toLocaleString()}</td>
                </tr>
              ))}
              {data.items.length === 0 && (
                <tr><td style={td} colSpan={5}>No tickets.</td></tr>
              )}
            </tbody>
          </table>

          <div style={{ display: 'flex', gap: 8, marginTop: 12, alignItems: 'center' }}>
            <button onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}>Prev</button>
            <span>Page {data.page} / {Math.max(1, Math.ceil(data.total / data.pageSize))}</span>
            <button
              onClick={() => setPage(p => p + 1)}
              disabled={data.page * data.pageSize >= data.total}
            >
              Next
            </button>
          </div>
        </>
      ) : null}
    </section>
  )
}

const th: React.CSSProperties = { textAlign: 'left', padding: 8, background: '#fafafa', borderBottom: '1px solid #ddd' }
const td: React.CSSProperties = { padding: 8 }
