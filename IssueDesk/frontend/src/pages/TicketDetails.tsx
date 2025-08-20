import { useParams, Link } from 'react-router-dom'
import { useState } from 'react'
import { useTicket } from '../api/queries'
import { useAssignTicket, useChangeTicketStatus, useAddComment } from '../api/mutations'
import { ChangeStatusBody, AddCommentBody, AssignBody } from '../api/schemas'

const statusOptions = ['New','InProgress','Resolved','Closed'] as const

export default function TicketDetails() {
  const { ticketId = '' } = useParams()
  const { data, isLoading, isError, error } = useTicket(ticketId)
  const [assignee, setAssignee] = useState('')
  const [nextStatus, setNextStatus] = useState<typeof statusOptions[number]>('InProgress')
  const [author, setAuthor] = useState('agent')
  const [body, setBody] = useState('')

  const projectId = data?.ticket.projectId
  const assignMutation = useAssignTicket(ticketId, projectId)
  const statusMutation = useChangeTicketStatus(ticketId, projectId)
  const commentMutation = useAddComment(ticketId)

  if (isLoading) return <p>Loading ticket…</p>
  if (isError) return <p style={{ color: 'crimson' }}>Error: {error?.message}</p>
  if (!data) return null

  const t = data.ticket

  return (
    <section>
      <p><Link to={`/projects/${t.projectId}`}>← Back to project</Link></p>
      <h2 style={{ marginBottom: 4 }}>{t.title}</h2>
      <p style={{ marginTop: 0, opacity: 0.75 }}>
        Status <b>{t.status}</b> • Priority <b>{t.priority}</b> • Assignee <b>{t.assignee ?? '-'}</b>
      </p>
      {t.description && <p style={{ whiteSpace: 'pre-wrap' }}>{t.description}</p>}

      <div style={{ display: 'grid', gap: 12, gridTemplateColumns: 'repeat(auto-fit,minmax(260px,1fr))', margin: '16px 0' }}>
        <form onSubmit={async (e) => {
          e.preventDefault()
          const payload: AssignBody = { assignee: assignee.trim() }
          if (!payload.assignee) return
          await assignMutation.mutateAsync(payload)
          setAssignee('')
        }}>
          <fieldset style={card}>
            <legend>Assign</legend>
            <input value={assignee} onChange={e => setAssignee(e.target.value)} placeholder="someone@desk.io" />
            <button type="submit" disabled={assignMutation.isPending} style={{ marginLeft: 8 }}>
              {assignMutation.isPending ? 'Assigning…' : 'Assign'}
            </button>
          </fieldset>
        </form>

        <form onSubmit={async (e) => {
          e.preventDefault()
          const payload: ChangeStatusBody = { nextStatus }
          await statusMutation.mutateAsync(payload)
        }}>
          <fieldset style={card}>
            <legend>Change status</legend>
            <select value={nextStatus} onChange={e => setNextStatus(e.target.value as typeof nextStatus)}>
              {statusOptions.map(s => <option key={s} value={s}>{s}</option>)}
            </select>
            <button type="submit" disabled={statusMutation.isPending} style={{ marginLeft: 8 }}>
              {statusMutation.isPending ? 'Updating…' : 'Update'}
            </button>
          </fieldset>
        </form>

        <form onSubmit={async (e) => {
          e.preventDefault()
          const payload: AddCommentBody = { author: author.trim(), body: body.trim() }
          if (!payload.author || payload.body.length < 3) return
          await commentMutation.mutateAsync(payload)
          setBody('')
        }}>
          <fieldset style={card}>
            <legend>Add comment</legend>
            <input value={author} onChange={e => setAuthor(e.target.value)} placeholder="author" />
            <textarea value={body} onChange={e => setBody(e.target.value)} placeholder="Write a comment…" rows={3} style={{ width: '100%', display: 'block', marginTop: 6 }} />
            <button type="submit" disabled={commentMutation.isPending} style={{ marginTop: 6 }}>
              {commentMutation.isPending ? 'Posting…' : 'Post'}
            </button>
          </fieldset>
        </form>
      </div>

      <h3>Comments</h3>
      {data.comments.length === 0 ? (
        <p>No comments yet.</p>
      ) : (
        <ul style={{ listStyle: 'none', padding: 0 }}>
          {data.comments.map(c => (
            <li key={c.id} style={{ borderTop: '1px solid #eee', padding: '8px 0' }}>
              <div style={{ fontWeight: 600 }}>{c.author}</div>
              <div style={{ whiteSpace: 'pre-wrap' }}>{c.body}</div>
              <div style={{ opacity: 0.6, fontSize: 12 }}>{new Date(c.createdAt).toLocaleString()}</div>
            </li>
          ))}
        </ul>
      )}
    </section>
  )
}

const card: React.CSSProperties = { padding: 8, border: '1px solid #ddd', borderRadius: 8 }
