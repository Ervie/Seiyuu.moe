#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENV_FILE="$SCRIPT_DIR/.env"

if [ ! -f "$ENV_FILE" ]; then
    echo "Error: .env file not found at $ENV_FILE"
    echo "Copy .env.template to .env and fill in your values."
    exit 1
fi

source "$ENV_FILE"

MISSING=()
[ -z "${DEPLOY_HOST:-}" ] && MISSING+=("DEPLOY_HOST")
[ -z "${DEPLOY_USER:-}" ] && MISSING+=("DEPLOY_USER")
[ -z "${DEPLOY_SSH_KEY:-}" ] && MISSING+=("DEPLOY_SSH_KEY")
[ -z "${DEPLOY_REMOTE_DIR:-}" ] && MISSING+=("DEPLOY_REMOTE_DIR")
[ -z "${SERVICE_NAME:-}" ] && MISSING+=("SERVICE_NAME")

if [ ${#MISSING[@]} -gt 0 ]; then
    echo "Error: Missing required variables in .env: ${MISSING[*]}"
    exit 1
fi

DEPLOY_SSH_KEY="${DEPLOY_SSH_KEY/#\~/$HOME}"

if [ ! -f "$DEPLOY_SSH_KEY" ]; then
    echo "Error: SSH key not found at $DEPLOY_SSH_KEY"
    exit 1
fi

SSH_OPTS="-i $DEPLOY_SSH_KEY -o StrictHostKeyChecking=accept-new"
SSH_TARGET="$DEPLOY_USER@$DEPLOY_HOST"

API_DIR="$SCRIPT_DIR/../SeiyuuMoe.API"
PUBLISH_DIR="$API_DIR/bin/Release/net10.0/publish"

echo "==> Publishing SeiyuuMoe.API..."
dotnet publish "$API_DIR/SeiyuuMoe.API.csproj" -c Release --self-contained false

if [ ! -d "$PUBLISH_DIR" ]; then
    echo "Error: Publish directory not found at $PUBLISH_DIR"
    exit 1
fi

echo "==> Syncing files to $SSH_TARGET:$DEPLOY_REMOTE_DIR..."
rsync -azv --delete \
    --exclude 'appsettings.json' \
    --exclude 'nlog.config' \
    --exclude 'Logs/' \
    -e "ssh $SSH_OPTS" \
    "$PUBLISH_DIR/" \
    "$SSH_TARGET:$DEPLOY_REMOTE_DIR/"

echo "==> Restarting $SERVICE_NAME service..."
ssh -t $SSH_OPTS "$SSH_TARGET" "sudo systemctl restart $SERVICE_NAME"

echo "==> Checking service status..."
ssh $SSH_OPTS "$SSH_TARGET" "systemctl is-active $SERVICE_NAME"

echo "==> Deploy complete."
