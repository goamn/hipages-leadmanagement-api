CREATE SEQUENCE suburbs_seq;

CREATE TABLE suburbs (
    id int check (id > 0) NOT NULL DEFAULT NEXTVAL ('suburbs_seq'),
    name varchar(255) NOT NULL,
    postcode varchar(4) NOT NULL,
    PRIMARY KEY (id)
) ;

CREATE INDEX idx_suburbs_postcode ON suburbs (postcode);

CREATE SEQUENCE categories_seq;

CREATE TABLE categories (
    id int check (id > 0) NOT NULL DEFAULT NEXTVAL ('categories_seq'),
    name varchar(255) NOT NULL,
    parent_category_id int NOT NULL DEFAULT '0',
    PRIMARY KEY (id)
) ;

CREATE INDEX idx_categories_parent_category ON categories (parent_category_id);

CREATE SEQUENCE jobs_seq;

CREATE TABLE jobs (
    id int check (id > 0) NOT NULL DEFAULT NEXTVAL ('jobs_seq'),
    status varchar(50) NOT NULL DEFAULT 'new',
    suburb_id int check (suburb_id > 0) NOT NULL,
    category_id int check (category_id > 0) NOT NULL,
    contact_name varchar(255) NOT NULL,
    contact_phone varchar(255) NOT NULL,
    contact_email varchar(255) NOT NULL,
    price int check (price > 0) NOT NULL,
    description text NOT NULL,
    created_at TIMESTAMP(0) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP(0) NOT NULL DEFAULT to_timestamp(0),
    PRIMARY KEY (id)
   ,
    CONSTRAINT fk_jobs_suburb FOREIGN KEY (suburb_id) REFERENCES suburbs (id),
    CONSTRAINT fk_jobs_category FOREIGN KEY (category_id) REFERENCES categories (id)
) ;

CREATE INDEX idx_jobs_suburb ON jobs (suburb_id);
CREATE INDEX idx_jobs_category ON jobs (category_id);
