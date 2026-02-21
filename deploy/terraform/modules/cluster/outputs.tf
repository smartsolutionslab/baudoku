output "cluster_id" {
  description = "ID of the DOKS cluster"
  value       = digitalocean_kubernetes_cluster.this.id
}

output "cluster_name" {
  description = "Name of the DOKS cluster"
  value       = digitalocean_kubernetes_cluster.this.name
}

output "endpoint" {
  description = "Kubernetes API endpoint"
  value       = digitalocean_kubernetes_cluster.this.endpoint
}

output "kubeconfig" {
  description = "Raw kubeconfig for the cluster"
  value       = digitalocean_kubernetes_cluster.this.kube_config[0]
  sensitive   = true
}
