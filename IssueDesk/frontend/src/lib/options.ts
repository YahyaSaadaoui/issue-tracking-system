export const STATUS_OPTIONS = [
  { label: "Any status", value: "all" },
  { label: "New", value: "New" },
  { label: "In progress", value: "InProgress" },
  { label: "Resolved", value: "Resolved" },
  { label: "Closed", value: "Closed" },
] as const;

export const PRIORITY_OPTIONS = [
  { label: "Any priority", value: "all" },
  { label: "Low", value: "Low" },
  { label: "Medium", value: "Medium" },
  { label: "High", value: "High" },
] as const;