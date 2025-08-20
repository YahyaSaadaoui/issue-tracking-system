import { Link } from 'react-router-dom'
import { useProjects } from '../api/queries'

export default function Projects() {
  const { data, isLoading, isError, error } = useProjects()

  if (isLoading) return <p>Loading projects…</p>
  if (isError) return <p style={{ color: 'crimson' }}>Error: {(error as Error).message}</p>
  if (!data?.length) return <p>No projects yet.</p>

  return (
    <section>
      <h2>Projects</h2>
      <ul style={{ paddingLeft: 16 }}>
        {data.map(p => (
          <li key={p.id} style={{ margin: '8px 0' }}>
            <Link to={`/projects/${p.id}`}>
              <strong>{p.name}</strong> <span style={{ opacity: 0.7 }}>[{p.key}]</span>
            </Link>
          </li>
        ))}
      </ul>
    </section>
  )
}
