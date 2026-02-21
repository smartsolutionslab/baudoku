#!/usr/bin/env bash
set -euo pipefail

# =============================================================================
# BauDoku Infrastructure Bootstrap
#
# One-time setup script that:
#   1. Creates the DO Spaces bucket for Terraform state
#   2. Provisions the shared DOKS cluster (nginx-ingress, cert-manager, ClusterIssuer)
#   3. Sets the required GitHub secrets
#
# Prerequisites:
#   - doctl (authenticated: doctl auth init)
#   - terraform >= 1.9
#   - gh (authenticated: gh auth login)
#   - DO Spaces API key (separate from main DO token)
#
# Usage:
#   ./bootstrap.sh
# =============================================================================

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REGION="fra1"
BUCKET="baudoku-terraform-state"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

info()  { echo -e "${GREEN}[INFO]${NC} $*"; }
warn()  { echo -e "${YELLOW}[WARN]${NC} $*"; }
error() { echo -e "${RED}[ERROR]${NC} $*" >&2; exit 1; }

# ---------------------------------------------------------------------------
# Check prerequisites
# ---------------------------------------------------------------------------
check_prerequisites() {
  info "Checking prerequisites..."

  for cmd in doctl terraform gh; do
    if ! command -v "$cmd" &>/dev/null; then
      error "'$cmd' is not installed. Please install it first."
    fi
  done

  # Verify doctl is authenticated
  if ! doctl account get &>/dev/null; then
    error "doctl is not authenticated. Run: doctl auth init"
  fi

  # Verify gh is authenticated
  if ! gh auth status &>/dev/null; then
    error "gh CLI is not authenticated. Run: gh auth login"
  fi

  info "All prerequisites met."
}

# ---------------------------------------------------------------------------
# Step 1: Create DO Spaces bucket
# ---------------------------------------------------------------------------
create_spaces_bucket() {
  info "Checking if Spaces bucket '${BUCKET}' exists..."

  if doctl spaces list --format Name --no-header | grep -q "^${BUCKET}$"; then
    info "Bucket '${BUCKET}' already exists. Skipping."
  else
    info "Creating Spaces bucket '${BUCKET}' in ${REGION}..."
    doctl spaces create "${BUCKET}" --region "${REGION}"
    info "Bucket created."
  fi
}

# ---------------------------------------------------------------------------
# Step 2: Collect and set secrets
# ---------------------------------------------------------------------------
collect_secrets() {
  info "Checking GitHub secrets..."

  echo ""
  echo "The following secrets are needed:"
  echo "  - DO_SPACES_ACCESS_KEY  (Spaces API key)"
  echo "  - DO_SPACES_SECRET_KEY  (Spaces API secret)"
  echo "  - KEYCLOAK_ADMIN_PASSWORD"
  echo ""
  echo "You can generate a Spaces key at:"
  echo "  https://cloud.digitalocean.com/account/api/spaces"
  echo ""

  read -rp "DO_SPACES_ACCESS_KEY: " DO_SPACES_ACCESS_KEY
  if [[ -z "$DO_SPACES_ACCESS_KEY" ]]; then
    error "DO_SPACES_ACCESS_KEY cannot be empty."
  fi

  read -rsp "DO_SPACES_SECRET_KEY: " DO_SPACES_SECRET_KEY
  echo ""
  if [[ -z "$DO_SPACES_SECRET_KEY" ]]; then
    error "DO_SPACES_SECRET_KEY cannot be empty."
  fi

  read -rsp "KEYCLOAK_ADMIN_PASSWORD: " KEYCLOAK_ADMIN_PASSWORD
  echo ""
  if [[ -z "$KEYCLOAK_ADMIN_PASSWORD" ]]; then
    error "KEYCLOAK_ADMIN_PASSWORD cannot be empty."
  fi

  read -rsp "ACME_EMAIL (for Let's Encrypt): " ACME_EMAIL
  echo ""
  if [[ -z "$ACME_EMAIL" ]]; then
    error "ACME_EMAIL cannot be empty."
  fi

  info "Setting GitHub secrets..."
  echo "$DO_SPACES_ACCESS_KEY"    | gh secret set DO_SPACES_ACCESS_KEY
  echo "$DO_SPACES_SECRET_KEY"    | gh secret set DO_SPACES_SECRET_KEY
  echo "$KEYCLOAK_ADMIN_PASSWORD" | gh secret set KEYCLOAK_ADMIN_PASSWORD

  info "GitHub secrets configured."

  # Export for Terraform
  export AWS_ACCESS_KEY_ID="$DO_SPACES_ACCESS_KEY"
  export AWS_SECRET_ACCESS_KEY="$DO_SPACES_SECRET_KEY"
  export TF_VAR_acme_email="$ACME_EMAIL"
}

# ---------------------------------------------------------------------------
# Step 3: Provision shared infrastructure (DOKS cluster)
# ---------------------------------------------------------------------------
provision_shared() {
  info "Provisioning shared infrastructure (DOKS cluster)..."

  cd "${SCRIPT_DIR}/environments/shared"

  info "Running terraform init..."
  terraform init

  info "Running terraform plan..."
  terraform plan -out=tfplan

  echo ""
  read -rp "Apply this plan? (yes/no): " CONFIRM
  if [[ "$CONFIRM" != "yes" ]]; then
    warn "Aborted. Run 'terraform apply tfplan' manually when ready."
    return
  fi

  info "Running terraform apply..."
  terraform apply tfplan
  rm -f tfplan

  info "Shared infrastructure provisioned."

  # Verify cluster
  CLUSTER_NAME=$(terraform output -raw cluster_name 2>/dev/null || echo "baudoku-k8s")
  info "Saving kubeconfig for cluster '${CLUSTER_NAME}'..."
  doctl kubernetes cluster kubeconfig save "${CLUSTER_NAME}"

  info "Verifying cluster nodes..."
  kubectl get nodes
}

# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------
main() {
  echo ""
  echo "=========================================="
  echo "  BauDoku Infrastructure Bootstrap"
  echo "=========================================="
  echo ""

  check_prerequisites
  create_spaces_bucket
  collect_secrets
  provision_shared

  echo ""
  info "Bootstrap complete!"
  echo ""
  echo "Next steps:"
  echo "  1. Push to 'development' branch → CI builds images → deploy workflow runs terraform apply → staging deployed"
  echo "  2. Merge to 'main' → same flow for production"
  echo ""
  echo "To manually deploy staging:"
  echo "  cd ${SCRIPT_DIR}/environments/staging"
  echo "  terraform init && terraform apply"
  echo ""
}

main "$@"
