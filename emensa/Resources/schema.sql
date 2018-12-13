-- Run as root
create database if not exists dbwt character set = utf8mb4 collate = utf8mb4_bin;

create role if not exists `dbwt_admin`;
create role if not exists `dbwt_user`;

grant all on dbwt.* to `dbwt_admin` with grant option;
grant select, insert, update, delete, execute on dbwt.* to `dbwt_user`;

create user if not exists 'dbwtAdmin'@'localhost' identified by 'securePwHere';
create user if not exists 'dbwtWebapp'@'localhost' identified by 'securePwHere';

grant `dbwt_admin` to 'dbwtAdmin'@'localhost';
grant `dbwt_user` to 'dbwtWebapp'@'localhost';

set default role `dbwt_admin` for 'dbwtAdmin'@'localhost';
set default role `dbwt_user` for 'dbwtWebapp'@'localhost';

use dbwt;

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
  salt       varchar(32)  not null,
  hash       varchar(24)  not null,
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
  matriculation_number int(9)  unsigned not null check (matriculation_number > 999999),
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
  guest_price    decimal(4, 2) not null check (guest_price >= employee_price),
  employee_price decimal(4, 2) default null check (employee_price <= guest_price and employee_price >= student_price),
  student_price  decimal(4, 2) default null check (student_price <= employee_price),
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

-- Procedures

drop procedure if exists add_guest;
drop procedure if exists add_student;
drop procedure if exists add_employee;
drop procedure if exists get_user_hash;
drop procedure if exists get_meal;

delimiter //
create procedure add_guest(id           int unsigned,
                           reason_      varchar(256),
                           valid_until_ date)
modifies sql data
  begin
    insert into guest values (id, reason_, valid_until_);
end//

create procedure add_student(id                    int unsigned,
                             matriculation_number_ int(9) unsigned,
                             major_                enum ('ET', 'INF', 'ISE', 'MCS', 'WI'))
modifies sql data
  begin
    replace into member values (id);
    insert into student values (id, matriculation_number_, major_);
end//

create procedure add_employee(id            int unsigned,
                              office_       varchar(4),
                              phone_number_ varchar(14))
modifies sql data
  begin
    replace into member values (id);
    insert into employee values (id, office_, phone_number_);
end//

create procedure get_user_hash(username_ varchar(32))
reads sql data
  begin
    select username, hash, salt from user where username = username_;
end//

create procedure get_meal(meal_id_ int unsigned)
reads sql data
  begin
    select m.name,
           m.description,
           i.file_path,
           ig.name ingredient_name,
           ig.organic,
           ig.gluten_free,
           ig.vegan,
           ig.vegetarian,
           p.employee_price,
           p.student_price,
           p.guest_price
    from meal m
           left join meal_image_relation mir on m.id = mir.meal_id
           left join image i on mir.image_id = i.id
           left join ingredient_meal_relation imr on m.id = imr.meal_id
           left join ingredient ig on imr.ingredient_id = ig.id
           left join price p on m.id = p.meal_id
    where m.id = meal_id_;
end//
delimiter ;
-- Functions

drop function if exists get_role;
drop function if exists add_user;

delimiter //
create function add_user(username_   varchar(32),
                         email_      varchar(128),
                         salt_       varchar(32),
                         hash_       varchar(24),
                         first_name_ varchar(32),
                         last_name_  varchar(64),
                         birthday_   date)
  returns int unsigned
modifies sql data
reads sql data
  begin
    declare user_id int unsigned;
    insert into user (username, email, salt, hash, first_name, last_name, birthday)
    values (username_, email_, salt_, hash_, first_name_, last_name_, birthday_);
    select id into user_id from user where username = username_;
    return user_id;
end//

create function get_role(username_ varchar(32))
  returns varchar(32)
  begin
    declare guest bool;
    declare student bool;
    declare employee bool;
    declare member bool;
    select g.user_id is not null,
           s.member_id is not null,
           e.member_id is not null,
           m.user_id is not null into guest, student, employee, member
    from user u
           left join member m on u.id = m.user_id
           left join guest g on u.id = g.user_id
           left join student s on m.user_id = s.member_id
           left join employee e on m.user_id = e.member_id
    where u.username = username_;
    if guest
    then return 'guest';
    elseif student
      then return 'student';
    elseif employee
      then return 'employee';
    elseif member
      then return 'member';
    else return 'no role';
    end if;
end//
delimiter ;

-- Views

drop view if exists products;
drop view if exists categories_with_parent;

create view products as
  select m.id                meal_id,
         m.available,
         m.name              meal_name,
         i.file_path,
         min(ig.organic)     organic,
         min(ig.gluten_free) gluten_free,
         min(ig.vegan)       vegan,
         min(ig.vegetarian)  vegetarian,
         c.id                category_id
  from meal m
         left join meal_image_relation mir on mir.meal_id = m.id
         left join image i on i.id = mir.image_id
         left join ingredient_meal_relation imr on m.id = imr.meal_id
         left join ingredient ig on ig.id = imr.ingredient_id
         left join category c on m.category_id = c.id
  group by meal_id;

create view categories_with_parent as
  select c.id, c.name, c2.id parent_id, c2.name parent_name
  from category c
         left join category c2 on c2.id = c.parent_category_id;

-- Data

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
values ('Bratrolle', 'Eine tolle Bratrolle, nennt man auch Frikandel.', 3, 7),
       ('Curry Wok', 'Eine asiatische Reispfanne, gebraten auf einem Wok.', 5, 6),
       ('Currywurst', 'Eine typische deutsche Currywurst. Dazu gibt es Pommes Frites.', 0, 7),
       ('Falafel', 'Eine Spezialität aus dem mittleren Osten. Wird aus Saubohnen und Kichererbsen gemacht', 7, 6),
       ('Käsestulle', 'Ein einfaches Brot mit Käse und Salat. Gut zum mitnehmen.', 2, 4),
       ('Krautsalat', 'Eine leckere Beilage oder Zwischenmahlzeit, auch für Vegetarier.', 8, 4),
       ('Schnitzel', 'Ein Wiener Schnitzel kommt immer gut and und schmeckt.', 1, 7),
       ('Spiegelei', 'Das typische Frühstück, hier mit Bacon-Streifen.', 0, 4);

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

insert into price
values (2018, 1, 3.5, 3.0, 2.5),
       (2018, 2, 3.5, 3.0, 2.5),
       (2018, 3, 3.5, 3.0, 2.5),
       (2018, 4, 3.5, 3.0, 2.5),
       (2018, 5, 3.5, 3.0, 2.5),
       (2018, 6, 3.5, 3.0, 2.5),
       (2018, 7, 3.5, 3.0, 2.5),
       (2018, 8, 3.5, 3.0, 2.5)