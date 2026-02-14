ALTER TABLE photos ADD COLUMN retry_count INTEGER DEFAULT 0;
ALTER TABLE photos ADD COLUMN last_upload_error TEXT;
