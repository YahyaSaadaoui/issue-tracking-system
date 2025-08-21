import axios from 'axios'
const baseURL = import.meta.env.VITE_API_BASE ?? '/api'
export const http = axios.create({ baseURL, headers: { 'Content-Type': 'application/json' } })
