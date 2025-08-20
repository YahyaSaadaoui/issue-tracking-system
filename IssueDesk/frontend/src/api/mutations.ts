import { useMutation, useQueryClient } from '@tanstack/react-query'
import { http } from './client'
import {
  AddCommentBody, AssignBody, ChangeStatusBody,
  CreateTicketInput, TicketDto
} from './schemas'

export function useCreateTicket(projectId: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (payload: CreateTicketInput) => {
      const { data } = await http.post('/tickets', payload)
      return TicketDto.parse(data)
    },
    onSuccess: () => {
      // refresh ticket list for this project
      qc.invalidateQueries({ queryKey: ['projectTickets', projectId] })
      qc.invalidateQueries({ queryKey: ['projects'] })
    },
  })
}

export function useAssignTicket(ticketId: string, projectId?: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (payload: AssignBody) => {
      await http.post(`/tickets/${ticketId}/assign`, payload)
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['ticket', ticketId] })
      if (projectId) qc.invalidateQueries({ queryKey: ['projectTickets', projectId] })
    },
  })
}

export function useChangeTicketStatus(ticketId: string, projectId?: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (payload: ChangeStatusBody) => {
      await http.post(`/tickets/${ticketId}/status`, payload)
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['ticket', ticketId] })
      if (projectId) qc.invalidateQueries({ queryKey: ['projectTickets', projectId] })
    },
  })
}

export function useAddComment(ticketId: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (payload: AddCommentBody) => {
      const { data } = await http.post(`/tickets/${ticketId}/comments`, payload)
      return data
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['ticket', ticketId] })
    },
  })
}
