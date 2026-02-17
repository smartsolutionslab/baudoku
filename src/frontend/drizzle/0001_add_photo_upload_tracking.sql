ALTER TABLE photos ADD COLUMN retry_count INTEGER DEFAULT 0;
--> statement-breakpoint
ALTER TABLE photos ADD COLUMN last_upload_error TEXT;
