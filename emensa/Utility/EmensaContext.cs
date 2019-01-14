using System.Linq;
using emensa.DataModels;
using emensa.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace emensa.Utility
{
    public partial class EmensaContext : DbContext
    {
        public EmensaContext()
        {
        }

        public EmensaContext(DbContextOptions<EmensaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Declaration> Declaration { get; set; }
        public virtual DbSet<DeclarationMealRelation> DeclarationMealRelation { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<Faculty> Faculty { get; set; }
        public virtual DbSet<FriendRelation> FriendRelation { get; set; }
        public virtual DbSet<Guest> Guest { get; set; }
        public virtual DbSet<Image> Image { get; set; }
        public virtual DbSet<Ingredient> Ingredient { get; set; }
        public virtual DbSet<IngredientMealRelation> IngredientMealRelation { get; set; }
        public virtual DbSet<Meal> Meal { get; set; }
        public virtual DbSet<MealImageRelation> MealImageRelation { get; set; }
        public virtual DbSet<Member> Member { get; set; }
        public virtual DbSet<MemberFacultyRelation> MemberFacultyRelation { get; set; }
        public virtual DbSet<DataModels.Order> Order { get; set; }
        public virtual DbSet<OrderMealRelation> OrderMealRelation { get; set; }
        public virtual DbSet<Price> Price { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<User> User { get; set; }

        public static User GetUser(string username)
        {
            using (var db = new EmensaContext())
            {
                return db.User.First(user => user.Username == username);
            }
        }
        
        public static Role GetRole(string username)
        {
            using (var db = new EmensaContext())
            {
                if ((from student in db.Student where student.Member.User.Username == username select student).ToList()
                    .Any())
                {
                    return Role.Student;
                }

                if ((from employee in db.Employee where employee.Member.User.Username == username select employee)
                    .ToList().Any())
                {
                    return Role.Employee;
                }

                return Role.Guest;
            }
        }

        public static bool DoesUserExist(string username)
        {
            using (var db = new EmensaContext())
            {
                return (from user in db.User where user.Username == username select user).ToList().Any();
            }
        }

        public static bool IsUserActivated(string username)
        {
            using (var db = new EmensaContext())
            {
                return DoesUserExist(username) &&
                       (from user in db.User where user.Username == username select user).ToList().First().Active != 0;
            }
        }

        public static bool IsPasswordCorrect(string username, string password)
        {
            using (var db = new EmensaContext())
            {
                var user = (from u in db.User where u.Username == username select u).ToList().First();
                return PasswordStorage.VerifyPassword(password, user.Salt, user.Hash);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("Server=localhost;Database=emensa;Uid=root;Pwd=password");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category", "emensa");

                entity.HasIndex(e => e.ImageId)
                    .HasName("image_id");

                entity.HasIndex(e => e.ParentCategoryId)
                    .HasName("parent_category_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.ImageId)
                    .HasColumnName("image_id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.ParentCategoryId)
                    .HasColumnName("parent_category_id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("NULL");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Category)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("category_ibfk_1");

                entity.HasOne(d => d.ParentCategory)
                    .WithMany(p => p.ChildCategories)
                    .HasForeignKey(d => d.ParentCategoryId)
                    .HasConstraintName("category_ibfk_2");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comment", "emensa");

                entity.HasIndex(e => e.MealId)
                    .HasName("meal_id");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.MealId)
                    .HasColumnName("meal_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(1024)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Rating)
                    .HasColumnName("rating")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Meal)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.MealId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("comment_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("comment_ibfk_1");
            });

            modelBuilder.Entity<Declaration>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("declaration", "emensa");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Label)
                    .IsRequired()
                    .HasColumnName("label")
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DeclarationMealRelation>(entity =>
            {
                entity.HasKey(e => new {DeclarationSymbol = e.DeclarationId, e.MealId});

                entity.ToTable("declaration_meal_relation", "emensa");

                entity.HasIndex(e => e.MealId)
                    .HasName("meal_id");

                entity.Property(e => e.DeclarationId)
                    .HasColumnName("declaration_id")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.MealId)
                    .HasColumnName("meal_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Declaration)
                    .WithMany(p => p.DeclarationMealRelation)
                    .HasForeignKey(d => d.DeclarationId)
                    .HasConstraintName("declaration_meal_relation_ibfk_1");

                entity.HasOne(d => d.Meal)
                    .WithMany(p => p.DeclarationMealRelation)
                    .HasForeignKey(d => d.MealId)
                    .HasConstraintName("declaration_meal_relation_ibfk_2");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.MemberId);

                entity.ToTable("employee", "emensa");

                entity.Property(e => e.MemberId)
                    .HasColumnName("member_id")
                    .HasColumnType("int(10) unsigned")
                    .ValueGeneratedNever();

                entity.Property(e => e.Office)
                    .HasColumnName("office")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("phone_number")
                    .HasMaxLength(14)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.HasOne(d => d.Member)
                    .WithOne(p => p.Employee)
                    .HasForeignKey<Employee>(d => d.MemberId)
                    .HasConstraintName("employee_ibfk_1");
            });

            modelBuilder.Entity<Faculty>(entity =>
            {
                entity.ToTable("faculty", "emensa");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnName("address")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.Website)
                    .IsRequired()
                    .HasColumnName("website")
                    .HasMaxLength(128)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<FriendRelation>(entity =>
            {
                entity.HasKey(e => new {e.FollowerId, e.FollowedId});

                entity.ToTable("friend_relation", "emensa");

                entity.HasIndex(e => e.FollowedId)
                    .HasName("followed_id");

                entity.Property(e => e.FollowerId)
                    .HasColumnName("follower_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.FollowedId)
                    .HasColumnName("followed_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.Following)
                    .HasForeignKey(d => d.FollowerId)
                    .HasConstraintName("friend_relation_ibfk_1");

                entity.HasOne(d => d.Followed)
                    .WithMany(p => p.Followers)
                    .HasForeignKey(d => d.FollowedId)
                    .HasConstraintName("friend_relation_ibfk_2");
            });

            modelBuilder.Entity<Guest>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("guest", "emensa");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned")
                    .ValueGeneratedNever();

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasColumnName("reason")
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.ValidUntil)
                    .HasColumnName("valid_until")
                    .HasColumnType("date")
                    .HasDefaultValueSql("(curdate() + interval 7 day)");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Guest)
                    .HasForeignKey<Guest>(d => d.UserId)
                    .HasConstraintName("guest_ibfk_1");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("image", "emensa");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.AlternativeText)
                    .IsRequired()
                    .HasColumnName("alternative_text")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasColumnName("file_path")
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.ToTable("ingredient", "emensa");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(5) unsigned")
                    .ValueGeneratedNever();

                entity.Property(e => e.GlutenFree)
                    .HasColumnName("gluten_free")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Organic)
                    .HasColumnName("organic")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Vegan)
                    .HasColumnName("vegan")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Vegetarian)
                    .HasColumnName("vegetarian")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");
            });

            modelBuilder.Entity<IngredientMealRelation>(entity =>
            {
                entity.HasKey(e => new {e.IngredientId, e.MealId});

                entity.ToTable("ingredient_meal_relation", "emensa");

                entity.HasIndex(e => e.MealId)
                    .HasName("meal_id");

                entity.Property(e => e.IngredientId)
                    .HasColumnName("ingredient_id")
                    .HasColumnType("int(5) unsigned");

                entity.Property(e => e.MealId)
                    .HasColumnName("meal_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.IngredientMealRelation)
                    .HasForeignKey(d => d.IngredientId)
                    .HasConstraintName("ingredient_meal_relation_ibfk_1");

                entity.HasOne(d => d.Meal)
                    .WithMany(p => p.IngredientMealRelation)
                    .HasForeignKey(d => d.MealId)
                    .HasConstraintName("ingredient_meal_relation_ibfk_2");
            });

            modelBuilder.Entity<Meal>(entity =>
            {
                entity.ToTable("meal", "emensa");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("category_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Available)
                    .HasColumnName("available")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(1024)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Stock)
                    .HasColumnName("stock")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Meal)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("meal_ibfk_1");
            });

            modelBuilder.Entity<MealImageRelation>(entity =>
            {
                entity.HasKey(e => new {e.MealId, e.ImageId});

                entity.ToTable("meal_image_relation", "emensa");

                entity.HasIndex(e => e.ImageId)
                    .HasName("image_id");

                entity.Property(e => e.MealId)
                    .HasColumnName("meal_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.ImageId)
                    .HasColumnName("image_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.MealImageRelation)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("meal_image_relation_ibfk_2");

                entity.HasOne(d => d.Meal)
                    .WithMany(p => p.MealImageRelation)
                    .HasForeignKey(d => d.MealId)
                    .HasConstraintName("meal_image_relation_ibfk_1");
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("member", "emensa");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned")
                    .ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Member)
                    .HasForeignKey<Member>(d => d.UserId)
                    .HasConstraintName("member_ibfk_1");
            });

            modelBuilder.Entity<MemberFacultyRelation>(entity =>
            {
                entity.HasKey(e => new {e.MemberId, e.FacultyId});

                entity.ToTable("member_faculty_relation", "emensa");

                entity.HasIndex(e => e.FacultyId)
                    .HasName("faculty_id");

                entity.Property(e => e.MemberId)
                    .HasColumnName("member_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.FacultyId)
                    .HasColumnName("faculty_id")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Faculty)
                    .WithMany(p => p.MemberFacultyRelation)
                    .HasForeignKey(d => d.FacultyId)
                    .HasConstraintName("member_faculty_relation_ibfk_2");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.MemberFacultyRelation)
                    .HasForeignKey(d => d.MemberId)
                    .HasConstraintName("member_faculty_relation_ibfk_1");
            });

            modelBuilder.Entity<DataModels.Order>(entity =>
            {
                entity.ToTable("order", "emensa");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.CollectedAt)
                    .HasColumnName("collected_at")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.OrderedAt)
                    .HasColumnName("ordered_at")
                    .HasDefaultValueSql("curtime()");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("NULL");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("order_ibfk_1");
            });

            modelBuilder.Entity<OrderMealRelation>(entity =>
            {
                entity.HasKey(e => new {e.OrderId, e.MealId});

                entity.ToTable("order_meal_relation", "emensa");

                entity.HasIndex(e => e.MealId)
                    .HasName("meal_id");

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.MealId)
                    .HasColumnName("meal_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Meal)
                    .WithMany(p => p.OrderMealRelation)
                    .HasForeignKey(d => d.MealId)
                    .HasConstraintName("order_meal_relation_ibfk_2");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderMealRelation)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("order_meal_relation_ibfk_1");
            });

            modelBuilder.Entity<Price>(entity =>
            {
                entity.HasKey(e => new {e.ValidYear, e.MealId});

                entity.ToTable("price", "emensa");

                entity.HasIndex(e => e.MealId)
                    .HasName("meal_id")
                    .IsUnique();

                entity.Property(e => e.ValidYear)
                    .HasColumnName("valid_year")
                    .HasColumnType("year(4)");

                entity.Property(e => e.MealId)
                    .HasColumnName("meal_id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.EmployeePrice)
                    .HasColumnName("employee_price")
                    .HasColumnType("decimal(4,2)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.GuestPrice)
                    .HasColumnName("guest_price")
                    .HasColumnType("decimal(4,2)");

                entity.Property(e => e.StudentPrice)
                    .HasColumnName("student_price")
                    .HasColumnType("decimal(4,2)")
                    .HasDefaultValueSql("NULL");

                entity.HasOne(d => d.Meal)
                    .WithOne(p => p.Price)
                    .HasForeignKey<Price>(d => d.MealId)
                    .HasConstraintName("price_ibfk_1");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.MemberId);

                entity.ToTable("student", "emensa");

                entity.Property(e => e.MemberId)
                    .HasColumnName("member_id")
                    .HasColumnType("int(10) unsigned")
                    .ValueGeneratedNever();

                entity.Property(e => e.Major)
                    .HasColumnName("major")
                    .HasColumnType("enum('ET','INF','ISE','MCD','WI')")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.MatriculationNumber)
                    .HasColumnName("matriculation_number")
                    .HasColumnType("int(9) unsigned");

                entity.HasOne(d => d.Member)
                    .WithOne(p => p.Student)
                    .HasForeignKey<Student>(d => d.MemberId)
                    .HasConstraintName("student_ibfk_1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user", "emensa");

                entity.HasIndex(e => e.Email)
                    .HasName("email")
                    .IsUnique();

                entity.HasIndex(e => e.Username)
                    .HasName("username")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Age)
                    .HasColumnName("age")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Birthday)
                    .HasColumnName("birthday")
                    .HasColumnType("date")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("date")
                    .HasDefaultValueSql("curdate()");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("first_name")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Hash)
                    .IsRequired()
                    .HasColumnName("hash")
                    .HasMaxLength(24)
                    .IsUnicode(false);

                entity.Property(e => e.LastLogin)
                    .HasColumnName("last_login")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("last_name")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasColumnName("salt")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });
        }
    }
}