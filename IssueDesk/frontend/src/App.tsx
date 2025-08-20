import { Link, Outlet } from 'react-router-dom'

export default function App() {
  return (
    <div style={{ maxWidth: 960, margin: '0 auto', padding: 16, fontFamily: 'system-ui, sans-serif' }}>
      <header style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <h1 style={{ margin: 0 }}>IssueDesk</h1>
        <nav style={{ display: 'flex', gap: 12 }}>
          <Link to="/">Projects</Link>
        </nav>
      </header>
      <Outlet />
    </div>
  )
}
