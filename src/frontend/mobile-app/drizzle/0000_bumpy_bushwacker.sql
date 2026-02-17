CREATE TABLE `installations` (
	`id` text PRIMARY KEY NOT NULL,
	`project_id` text NOT NULL,
	`zone_id` text NOT NULL,
	`type` text NOT NULL,
	`status` text NOT NULL,
	`gps_lat` real,
	`gps_lng` real,
	`gps_altitude` real,
	`gps_altitude_msl` real,
	`gps_accuracy` real,
	`gps_source` text,
	`gps_corr_service` text,
	`gps_rtk_status` text,
	`gps_sat_count` integer,
	`gps_hdop` real,
	`gps_corr_age` real,
	`depth_mm` integer,
	`position_on_plan` text,
	`manufacturer` text,
	`model` text,
	`serial_number` text,
	`cable_type` text,
	`cross_section_mm2` real,
	`length_m` real,
	`circuit_id` text,
	`fuse_type` text,
	`fuse_rating_a` real,
	`voltage_v` integer,
	`phase` text,
	`notes` text,
	`installed_by` text,
	`installed_at` integer,
	`inspected_by` text,
	`inspected_at` integer,
	`created_at` integer NOT NULL,
	`updated_at` integer NOT NULL,
	`version` integer DEFAULT 1 NOT NULL,
	FOREIGN KEY (`project_id`) REFERENCES `projects`(`id`) ON UPDATE no action ON DELETE no action,
	FOREIGN KEY (`zone_id`) REFERENCES `zones`(`id`) ON UPDATE no action ON DELETE no action
);
--> statement-breakpoint
CREATE TABLE `measurements` (
	`id` text PRIMARY KEY NOT NULL,
	`installation_id` text NOT NULL,
	`type` text NOT NULL,
	`value` real NOT NULL,
	`unit` text NOT NULL,
	`min_threshold` real,
	`max_threshold` real,
	`result` text,
	`notes` text,
	`measured_at` integer NOT NULL,
	`measured_by` text NOT NULL,
	`version` integer DEFAULT 1 NOT NULL,
	FOREIGN KEY (`installation_id`) REFERENCES `installations`(`id`) ON UPDATE no action ON DELETE no action
);
--> statement-breakpoint
CREATE TABLE `photos` (
	`id` text PRIMARY KEY NOT NULL,
	`installation_id` text NOT NULL,
	`local_path` text NOT NULL,
	`remote_path` text,
	`thumbnail_path` text,
	`type` text NOT NULL,
	`annotations` text,
	`gps_lat` real,
	`gps_lng` real,
	`gps_accuracy` real,
	`gps_source` text,
	`gps_corr_service` text,
	`taken_at` integer NOT NULL,
	`upload_status` text NOT NULL,
	`version` integer DEFAULT 1 NOT NULL,
	FOREIGN KEY (`installation_id`) REFERENCES `installations`(`id`) ON UPDATE no action ON DELETE no action
);
--> statement-breakpoint
CREATE TABLE `projects` (
	`id` text PRIMARY KEY NOT NULL,
	`name` text NOT NULL,
	`street` text,
	`city` text,
	`zip_code` text,
	`gps_lat` real,
	`gps_lng` real,
	`client_name` text,
	`client_contact` text,
	`status` text NOT NULL,
	`created_by` text NOT NULL,
	`created_at` integer NOT NULL,
	`updated_at` integer NOT NULL,
	`version` integer DEFAULT 1 NOT NULL
);
--> statement-breakpoint
CREATE TABLE `sync_meta` (
	`key` text PRIMARY KEY NOT NULL,
	`value` text NOT NULL
);
--> statement-breakpoint
CREATE TABLE `sync_outbox` (
	`id` text PRIMARY KEY NOT NULL,
	`entity_type` text NOT NULL,
	`entity_id` text NOT NULL,
	`operation` text NOT NULL,
	`payload` text NOT NULL,
	`timestamp` integer NOT NULL,
	`device_id` text NOT NULL,
	`retry_count` integer DEFAULT 0,
	`status` text NOT NULL
);
--> statement-breakpoint
CREATE TABLE `zones` (
	`id` text PRIMARY KEY NOT NULL,
	`project_id` text NOT NULL,
	`parent_zone_id` text,
	`name` text NOT NULL,
	`type` text NOT NULL,
	`qr_code` text,
	`sort_order` integer DEFAULT 0,
	`version` integer DEFAULT 1 NOT NULL,
	FOREIGN KEY (`project_id`) REFERENCES `projects`(`id`) ON UPDATE no action ON DELETE no action
);
