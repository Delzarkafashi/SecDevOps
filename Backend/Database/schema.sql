CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS citext;

-- USERS
CREATE TABLE IF NOT EXISTS users (
  id BIGSERIAL PRIMARY KEY,
  email CITEXT NOT NULL UNIQUE,
  username TEXT UNIQUE,
  first_name TEXT,
  last_name TEXT,
  password_hash TEXT NOT NULL,
  role TEXT NOT NULL,
  is_active BOOLEAN NOT NULL DEFAULT true,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  last_login_at TIMESTAMPTZ,
  failed_login_count INT NOT NULL DEFAULT 0,
  locked_until TIMESTAMPTZ
);

ALTER TABLE users
DROP CONSTRAINT IF EXISTS ck_users_role;

ALTER TABLE users
ADD CONSTRAINT ck_users_role
CHECK (role IN ('viewer','staff','admin'));

ALTER TABLE users
DROP CONSTRAINT IF EXISTS ck_users_failed_login_count;

ALTER TABLE users
ADD CONSTRAINT ck_users_failed_login_count
CHECK (failed_login_count >= 0 AND failed_login_count <= 10);


-- POSTS
DROP TABLE IF EXISTS posts;

CREATE TABLE posts (
  id BIGSERIAL PRIMARY KEY,
  title TEXT NOT NULL,
  content TEXT NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  updated_at TIMESTAMPTZ,
  created_by BIGINT NOT NULL,
  updated_by BIGINT,

  CONSTRAINT ck_posts_title_not_empty CHECK (length(trim(title)) >= 1),
  CONSTRAINT ck_posts_content_not_empty CHECK (length(trim(content)) >= 1),
  CONSTRAINT ck_posts_title_len CHECK (length(title) <= 120),

  CONSTRAINT fk_posts_created_by
    FOREIGN KEY (created_by) REFERENCES users(id) ON DELETE RESTRICT,

  CONSTRAINT fk_posts_updated_by
    FOREIGN KEY (updated_by) REFERENCES users(id) ON DELETE SET NULL
);


-- TRIGGER updated_at
CREATE OR REPLACE FUNCTION set_updated_at()
RETURNS TRIGGER AS $$
BEGIN
  NEW.updated_at = now();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_posts_updated_at ON posts;

CREATE TRIGGER trg_posts_updated_at
BEFORE UPDATE ON posts
FOR EACH ROW
EXECUTE FUNCTION set_updated_at();


-- INDEX
CREATE INDEX IF NOT EXISTS idx_users_role ON users (role);
CREATE INDEX IF NOT EXISTS idx_posts_created_by ON posts (created_by);
