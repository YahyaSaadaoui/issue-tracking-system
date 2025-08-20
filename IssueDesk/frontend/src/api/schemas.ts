import { z } from 'zod'

export const TicketStatus = z.enum(['New', 'InProgress', 'Resolved', 'Closed'])
export const TicketPriority = z.enum(['Low', 'Medium', 'High'])

export const ProjectDto = z.object({
  id: z.string().uuid(),
  name: z.string(),
  key: z.string(),
  createdAt: z.string(),
})
export type ProjectDto = z.infer<typeof ProjectDto>

export const TicketDto = z.object({
  id: z.string().uuid(),
  projectId: z.string().uuid(),
  title: z.string(),
  description: z.string().nullable().optional(),
  status: TicketStatus,
  priority: TicketPriority,
  assignee: z.string().nullable().optional(),
  createdAt: z.string(),
  updatedAt: z.string(),
})
export type TicketDto = z.infer<typeof TicketDto>

export const PagedTickets = z.object({
  items: z.array(TicketDto),
  total: z.number(),
  page: z.number(),
  pageSize: z.number(),
})
export type PagedTickets = z.infer<typeof PagedTickets>

export const CommentDto = z.object({
  id: z.string().uuid(),
  author: z.string(),
  body: z.string(),
  createdAt: z.string(),
})
export type CommentDto = z.infer<typeof CommentDto>

export const TicketDetailsResponse = z.object({
  ticket: TicketDto,
  comments: z.array(CommentDto),
})
export type TicketDetailsResponse = z.infer<typeof TicketDetailsResponse>

export const CreateTicketInput = z.object({
  projectId: z.string().uuid(),
  title: z.string().min(5).max(120),
  description: z.string().max(5000).optional(),
  priority: TicketPriority,
})
export type CreateTicketInput = z.infer<typeof CreateTicketInput>

export const AssignBody = z.object({
  assignee: z.string().min(1).max(200),
})
export type AssignBody = z.infer<typeof AssignBody>

export const ChangeStatusBody = z.object({
  nextStatus: TicketStatus,
})
export type ChangeStatusBody = z.infer<typeof ChangeStatusBody>

export const AddCommentBody = z.object({
  author: z.string().min(1).max(200),
  body: z.string().min(3).max(5000),
})
export type AddCommentBody = z.infer<typeof AddCommentBody>