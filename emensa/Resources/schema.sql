create database if not exists emensa;
use emensa;

drop table if exists order_meal_relation;
drop table if exists member_faculty_relation;
drop table if exists meal_image_relation;
drop table if exists ingredient_meal_relation;
drop table if exists friend_relation;
drop table if exists declaration_meal_relation;
drop table if exists price;
drop table if exists comment;
drop table if exists student;
drop table if exists employee;
drop table if exists member;
drop table if exists guest;
drop table if exists meal;
drop table if exists category;
drop table if exists `order`;
drop table if exists ingredient;
drop table if exists declaration;
drop table if exists faculty;
drop table if exists image;
drop table if exists user;

create table user (
  id         int unsigned not null primary key auto_increment,
  username   varchar(32)  not null unique,
  email      varchar(128) not null unique,
  salt       varchar(32)  not null             default 'salt',
  hash       varchar(24)  not null             default 'hash',
  first_name varchar(32)  not null,
  last_name  varchar(64)  not null,
  created    date         not null             default current_date,
  active     boolean      not null             default false,
  birthday   date                              default null,
  last_login datetime                          default null,
  age        int unsigned                      as (datediff(current_date, birthday) div 365)
);

create table image (
  id               int unsigned not null primary key auto_increment,
  alternative_text varchar(64)  not null,
  file_path        varchar(512) not null,
  title            varchar(32)                       default null
);

create table faculty (
  id      int unsigned not null primary key auto_increment,
  website varchar(128) not null,
  name    varchar(128) not null,
  address varchar(128) not null
);

create table declaration (
  symbol varchar(2)  not null primary key,
  label  varchar(32) not null
);

create table ingredient (
  id          int(5) unsigned not null primary key,
  name        varchar(64)     not null,
  organic     boolean         not null             default false,
  vegetarian  boolean         not null             default false,
  vegan       boolean         not null             default false,
  gluten_free boolean         not null             default false
);

create table `order` (
  id           int unsigned not null primary key auto_increment,
  orderd_at    datetime     not null             default current_time,
  collected_at datetime                          default null check (collected_at > orderd_at),
  user_id      int unsigned,
  foreign key (user_id) references user (id)
    on delete set null
);

create table category (
  id                 int unsigned not null primary key auto_increment,
  name               varchar(32)  not null,
  image_id           int unsigned,
  parent_category_id int unsigned,
  foreign key (image_id) references image (id)
    on delete set null,
  foreign key (parent_category_id) references category (id)
    on delete set null
);

create table meal (
  id          int unsigned  not null primary key auto_increment,
  name        varchar(64)   not null,
  description varchar(1024) not null,
  stock       int unsigned  not null             default 0,
  category_id int unsigned  not null,
  available   bool                               as (stock > 0),
  foreign key (category_id) references category (id)
    on delete cascade
);

create table guest (
  user_id     int unsigned not null unique,
  reason      varchar(256) not null,
  valid_until date         not null default date_add(current_date, interval 7 day),
  foreign key (user_id) references user (id)
    on delete cascade
);

create table member (
  user_id int unsigned not null unique,
  foreign key (user_id) references user (id)
    on delete cascade
);

create table employee (
  member_id    int unsigned not null unique,
  office       varchar(4)  default null,
  phone_number varchar(14) default null check (phone_number is null or
                                               length(phone_number) between 13 and 14),
  foreign key (member_id) references member (user_id)
    on delete cascade
);

create table student (
  member_id            int unsigned     not null unique,
  matriculation_number int(9)  unsigned not null check (matriculation_number > 9999999),
  major                enum ('ET', 'INF', 'ISE', 'MCD', 'WI'),
  foreign key (member_id) references member (user_id)
    on delete cascade
);

create table comment (
  id      int unsigned not null primary key auto_increment,
  note    varchar(1024)                     default null,
  rating  int unsigned not null check (rating between 1 and 5),
  user_id int unsigned not null,
  meal_id int unsigned not null,
  foreign key (user_id) references student (member_id)
    on delete no action,
  foreign key (meal_id) references meal (id)
    on delete no action
);

create table price (
  valid_year     year          not null,
  meal_id        int unsigned  not null unique,
  guest_price    decimal(2, 2) not null check (guest_price >= employee_price),
  employee_price decimal(2, 2) default null check (employee_price between guest_price and student_price),
  student_price  decimal(2, 2) default null check (student_price <= employee_price),
  foreign key (meal_id) references meal (id)
    on delete cascade
);

create table friend_relation (
  initiator int unsigned not null,
  receiver  int unsigned not null,
  unique (initiator, receiver),
  foreign key (initiator) references user (id)
    on delete cascade,
  foreign key (receiver) references user (id)
    on delete cascade
);

create table order_meal_relation (
  amount   int unsigned not null,
  order_id int unsigned not null,
  meal_id  int unsigned not null,
  unique (order_id, meal_id),
  foreign key (order_id) references `order` (id)
    on delete cascade,
  foreign key (meal_id) references meal (id)
    on delete cascade
);

create table member_faculty_relation (
  member_id  int unsigned not null,
  faculty_id int unsigned not null,
  unique (member_id, faculty_id),
  foreign key (member_id) references member (user_id)
    on delete cascade,
  foreign key (faculty_id) references faculty (id)
    on delete cascade
);

create table declaration_meal_relation (
  declaration_symbol varchar(2)   not null,
  meal_id            int unsigned not null,
  unique (declaration_symbol, meal_id),
  foreign key (declaration_symbol) references declaration (symbol)
    on delete cascade,
  foreign key (meal_id) references meal (id)
    on delete cascade
);

create table ingredient_meal_relation (
  ingredient_id int(5) unsigned not null,
  meal_id       int unsigned    not null,
  unique (ingredient_id, meal_id),
  foreign key (ingredient_id) references ingredient (id)
    on delete cascade,
  foreign key (meal_id) references meal (id)
    on delete cascade
);

create table meal_image_relation (
  meal_id  int unsigned not null,
  image_id int unsigned not null,
  unique (meal_id, image_id),
  foreign key (meal_id) references meal (id)
    on delete cascade,
  foreign key (image_id) references image (id)
    on delete cascade
);

insert into image (alternative_text, file_path)
values ('Bratrolle', 'bratrolle.jpg'),
       ('Curry Wok', 'curry_wok.jpg'),
       ('Currywurst', 'currywurst.jpg'),
       ('Falafel', 'falafel.jpg'),
       ('Käsestulle', 'kaesestulle.jpg'),
       ('Krautsalat', 'krautsalat.jpg'),
       ('Schnitzel', 'schnitzel.jpg'),
       ('Spiegelei', 'spiegelei.jpg');

insert into category (name, parent_category_id, image_id)
values ('Hauptspeisen', NULL, NULL),
       ('Kleinigkeiten', NULL, NULL),
       ('Smoothies', 2, NULL),
       ('Snacks', 2, NULL),
       ('Burger und Co', 1, NULL),
       ('Asiatisch', 1, NULL),
       ('Klassiker', 1, NULL),
       ('Italienisch', 1, NULL),
       ('Aktionen', NULL, NULL),
       ('Weihnachten', 9, NULL),
       ('Sommergenuss', 9, NULL),
       ('Mensa Vital', 9, NULL),
       ('Sonderangebote', NULL, NULL),
       ('Ersti-Woche', 13, NULL),
       ('Geburtstagsessen', 13, NULL);

insert into meal (name, description, stock, category_id)
values ('Bratrolle', '', 3, 7),
       ('Curry Wok', '', 5, 6),
       ('Currywurst', '', 0, 7),
       ('Falafel', '', 7, 6),
       ('Käsestulle', '', 2, 4),
       ('Krautsalat', '', 8, 4),
       ('Schnitzel', '', 1, 7),
       ('Spiegelei', '', 0, 4);

insert into meal_image_relation (meal_id, image_id)
values (1, 1),
       (2, 2),
       (3, 3),
       (4, 4),
       (5, 5),
       (6, 6),
       (7, 7),
       (8, 8);

insert into declaration (symbol, label)
values ('2', 'Konservierungsstoff'),
       ('3', 'Antioxidationsmittel'),
       ('4', 'Geschmacksverstärker'),
       ('5', 'geschwefelt'),
       ('6', 'geschwärzt'),
       ('7', 'gewachst'),
       ('8', 'Phosphat'),
       ('9', 'Süßungsmittel'),
       ('10', 'enthält eine Phenylalaninquelle'),
       ('A', 'Gluten'),
       ('A1', 'Weizen'),
       ('A2', 'Roggen'),
       ('A3', 'Gerste'),
       ('A4', 'Hafer'),
       ('A5', 'Dinkel'),
       ('B', 'Sellerie'),
       ('C', 'Krebstiere'),
       ('D', 'Eier'),
       ('E', 'Fische'),
       ('F', 'Erdnüsse'),
       ('G', 'Sojabohnen'),
       ('H', 'Milch'),
       ('I', 'Schalenfrüchte'),
       ('I1', 'Mandeln'),
       ('I2', 'Haselnüsse'),
       ('I3', 'Walnüsse'),
       ('I4', 'Kaschunüsse'),
       ('I5', 'Pecannüsse'),
       ('I6', 'Paranüsse'),
       ('I7', 'Pistazien'),
       ('I8', 'Macadamianüsse'),
       ('J', 'Senf'),
       ('K', 'Sesamsamen'),
       ('L', 'Schwefeldioxid oder Sulfite'),
       ('M', 'Lupinen'),
       ('N', 'Weichtiere');

insert into ingredient (id, name, organic, vegetarian, vegan, gluten_free)
values (80, 'Aal', 0, 0, 0, 1),
       (81, 'Forelle', 0, 0, 0, 1),
       (82, 'Barsch', 0, 0, 0, 1),
       (83, 'Lachs', 0, 0, 0, 1),
       (84, 'Lachs', 1, 0, 0, 1),
       (85, 'Heilbutt', 0, 0, 0, 1),
       (86, 'Heilbutt', 1, 0, 0, 1),
       (100, 'Kurkumin', 1, 1, 1, 1),
       (101, 'Riboflavin', 0, 1, 1, 1),
       (123, 'Amaranth', 1, 1, 1, 1),
       (150, 'Zuckerkulör', 0, 1, 1, 1),
       (171, 'Titandioxid', 0, 1, 1, 1),
       (220, 'Schwefeldioxid', 0, 1, 1, 1),
       (270, 'Milchsäure', 0, 1, 1, 1),
       (322, 'Lecithin', 0, 1, 1, 1),
       (330, 'Zitronensäure', 1, 1, 1, 1),
       (999, 'Weizenmehl', 1, 1, 1, 0),
       (1000, 'Weizenmehl', 0, 1, 1, 0),
       (1001, 'Hanfmehl', 1, 1, 1, 1),
       (1010, 'Zucker', 0, 1, 1, 1),
       (1013, 'Traubenzucker', 0, 1, 1, 1),
       (1015, 'Branntweinessig', 0, 1, 1, 1),
       (2019, 'Karotten', 0, 1, 1, 1),
       (2020, 'Champignons', 0, 1, 1, 1),
       (2101, 'Schweinefleisch', 0, 0, 0, 1),
       (2102, 'Speck', 0, 0, 0, 1),
       (2103, 'Alginat', 0, 1, 1, 1),
       (2105, 'Paprika', 0, 1, 1, 1),
       (2107, 'Fenchel', 0, 1, 1, 1),
       (2108, 'Sellerie', 0, 1, 1, 1),
       (9020, 'Champignons', 1, 1, 1, 1),
       (9105, 'Paprika', 1, 1, 1, 1),
       (9107, 'Fenchel', 1, 1, 1, 1),
       (9110, 'Sojasprossen', 1, 1, 1, 1);

insert into faculty (id, website, name, address)
values (1, 'Architektur', 'https://www.fh-aachen.de/fachbereiche/architektur/', 'Bayernallee 9, 52066 Aachen'),
       (2,
        'Bauingenieurwesen',
        'https://www.fh-aachen.de/fachbereiche/bauingenieurwesen/',
        'Bayernallee 9, 52066 Aachen'),
       (3,
        'Chemie und Biotechnologie',
        'https://www.fh-aachen.de/fachbereiche/chemieundbiotechnologie/',
        'Heinrich-Mußmann-Straße 1, 52428 Jülich'),
       (4, 'Gestaltung', 'https://www.fh-aachen.de/fachbereiche/gestaltung/', 'Boxgraben 100, 52064 Aachen'),
       (5,
        'Elektrotechnik und Informationstechnik',
        'https://www.fh-aachen.de/fachbereiche/elektrotechnik-und-informationstechnik/',
        'Eupener Straße 70, 52066 Aachen'),
       (6,
        'Luft- und Raumfahrttechnik',
        'https://www.fh-aachen.de/fachbereiche/luft-und-raumfahrttechnik/',
        'Hohenstaufenallee 6, 52064 Aachen'),
       (7,
        'Wirtschaftswissenschaften',
        'https://www.fh-aachen.de/fachbereiche/wirtschaft/',
        'Eupener Straße 70, 52066 Aachen'),
       (8,
        'Maschinenbau und Mechatronik',
        'https://www.fh-aachen.de/fachbereiche/maschinenbau-und-mechatronik/',
        'Goethestraße 1, 52064 Aachen'),
       (9,
        'Medizintechnik und Technomathematik',
        'https://www.fh-aachen.de/fachbereiche/medizintechnik-und-technomathematik/',
        'Heinrich-Mußmann-Straße 1, 52428 Jülich'),
       (10,
        'Energietechnik',
        'https://www.fh-aachen.de/fachbereiche/energietechnik/',
        'Heinrich-Mußmann-Straße 1, 52428 Jülich');

insert into user (username, email, first_name, last_name)
values ('user1', 'user1@mail.com', 'user', 'name'),
       ('user2', 'user2@mail.com', 'user', 'name'),
       ('user3', 'user3@mail.com', 'user', 'name'),
       ('user4', 'user4@mail.com', 'user', 'name');

insert into member (user_id)
values (1),
       (2),
       (3);

insert into student (member_id, matriculation_number, major)
values (1, '11111111', 'INF'),
       (2, '111111111', 'ET');

insert into employee (member_id)
values (3);

INSERT INTO ingredient_meal_relation (meal_id, ingredient_id)
VALUES (2, 9110),
       (2, 2105),
       (2, 2101), -- Curry wok
       (7, 2101),
       (7, 1000),
       (7, 270),  -- Schnitzel
       (1, 2102),
       (1, 2105),
       (1, 1000), -- Bratrolle
       (6, 1010),
       (6, 2019),
       (6, 100),  -- Krautsalat
       (4, 2019),
       (4, 100),
       (4, 2108), -- Falafel
       (3, 2102),
       (3, 2019),
       (3, 1000), -- Currywurst
       (5, 2105),
       (5, 9020),
       (5, 1000), -- Käsestulle
       (8, 270),
       (8, 2102),
       (8, 9020); -- Spiegelei


insert into user (id, first_name, last_name, email, username, last_login, created, birthday, salt, hash, active)
values (21,
        'Bugs',
        'Findmore',
        'dbwt2018@ismypassword.com',
        'bugfin',
        '2018-11-14 17:44:10',
        '2018-11-14',
        '1996-12-13',
        'MPVdLDf0zNVzpOHP+GmRxoBg9mdJIlc5',
        '4nx5U6DIE+N8xsbpwUr3Q1KG',
        1),
       (22,
        'Donald',
        'Truck',
        'testing@ismypassword.com',
        'dot',
        '2018-11-14 17:44:10',
        '2018-11-14',
        '1991-12-11',
        'Ydn1iGl08JvvkVExSEiKDQhfYOaCtgOO',
        'm5kZ68YVNU3xBiDqorthK9UP',
        1),
       (23,
        'Fiona',
        'Index',
        'an0ther@ismypassword.com',
        'fionad',
        '2018-11-14 17:44:10',
        '2018-11-14',
        '1993-12-10',
        'I5GXy7BwYU2t3pHZ5YkBfKMbvN7Sr81O',
        'oYylNvPe7YmjO1IHNdLA/XxJ',
        1),
       (24,
        'Wendy',
        'Burger',
        's3cr3tz@ismypassword.com',
        'bkahuna',
        '2018-11-14 17:44:10',
        '2018-11-14',
        '1982-12-12',
        't1TAVguVwIiejXf3baaObIAtPx7Y+2iY',
        'IMK2n5r8RUVFo4bMMS8uDyH4',
        1),
       (25,
        'Monster',
        'Robot',
        '^;_`;^@ismypassword.com',
        'root',
        '2018-11-14 17:44:10',
        '2018-11-14',
        '1982-12-12',
        'dX8YsBM9atpYto9caWHJM6Eet7bUngxk',
        'nRt3MSBdNUHPj/q02WPgXaDA',
        1);

insert into friend_relation
values (21, 22),
       (21, 23),
       (21, 24),
       (22, 23),
       (22, 24);

create procedure get_user_role(in user_id int, out)
