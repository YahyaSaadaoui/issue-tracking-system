import { useQuery } from '@tanstack/react-query'
import type { UseQueryResult } from '@tanstack/react-query'
import { http } from './client'
import { PagedTickets, ProjectDto } from './schemas'
import type { PagedTickets as PagedTicketsT, ProjectDto as ProjectDtoT } from './schemas'
import { TicketDetailsResponse } from './schemas'


export function useProjects(): UseQueryResult<ProjectDtoT[], Error> {
  return useQuery<ProjectDtoT[], Error>({
    queryKey: ['projects'],
    queryFn: async () => {
      const { data } = await http.get('/projects')
      const arr = Array.isArray(data) ? data : []
      return arr.map((x) => ProjectDto.parse(x))
    },
  })
}

export interface TicketFilters {
  status?: string
  priority?: string
  assignee?: string
  search?: string
  page?: number
  pageSize?: number
}

export function useProjectTickets(
  projectId: string,
  filters: TicketFilters
): UseQueryResult<PagedTicketsT, Error> {
  const params = new URLSearchParams()
  if (filters.status) params.set('status', filters.status)
  if (filters.priority) params.set('priority', filters.priority)
  if (filters.assignee) params.set('assignee', filters.assignee)
  if (filters.search) params.set('search', filters.search)
  params.set('page', String(filters.page ?? 1))
  params.set('pageSize', String(filters.pageSize ?? 10))

  return useQuery<PagedTicketsT, Error>({
    queryKey: ['projectTickets', projectId, Object.fromEntries(params)],
    queryFn: async () => {
      const { data } = await http.get(`/projects/${projectId}/tickets?${params.toString()}`)
      return PagedTickets.parse(data)
    },
    enabled: !!projectId,
    
    staleTime: 5_000,
  })
}

export function useTicket(ticketId: string): UseQueryResult<TicketDetailsResponse, Error> {
  return useQuery<TicketDetailsResponse, Error>({
    queryKey: ['ticket', ticketId],
    queryFn: async () => {
      const { data } = await http.get(`/tickets/${ticketId}`)
      return TicketDetailsResponse.parse(data)
    },
    enabled: !!ticketId,
    staleTime: 5_000,
  })
}
