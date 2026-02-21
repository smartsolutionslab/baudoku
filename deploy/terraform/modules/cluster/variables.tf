variable "cluster_name" {
  description = "Name of the DOKS cluster"
  type        = string
  default     = "baudoku-k8s"
}

variable "region" {
  description = "DigitalOcean region"
  type        = string
  default     = "fra1"
}

variable "k8s_version" {
  description = "Kubernetes version prefix (latest patch is auto-selected)"
  type        = string
  default     = "1.32"
}

variable "node_size" {
  description = "Droplet size for worker nodes"
  type        = string
  default     = "s-2vcpu-4gb"
}

variable "min_nodes" {
  description = "Minimum number of nodes in the auto-scaling pool"
  type        = number
  default     = 2
}

variable "max_nodes" {
  description = "Maximum number of nodes in the auto-scaling pool"
  type        = number
  default     = 5
}

variable "acme_email" {
  description = "Email address for Let's Encrypt certificate notifications"
  type        = string
}
