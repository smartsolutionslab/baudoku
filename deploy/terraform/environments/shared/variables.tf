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

variable "acme_email" {
  description = "Email address for Let's Encrypt certificate notifications"
  type        = string
}
