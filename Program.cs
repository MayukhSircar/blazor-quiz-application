using CSharpQuizBlazor.Components;
using CSharpQuizBlazor.Models;
using CSharpQuizBlazor.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Use AddDbContextFactory with proper configuration
builder.Services.AddDbContextFactory<QuizDbContext>(options =>
{
    string connectionString;
    if (builder.Environment.IsProduction())
    {
        // Azure App Service persistent storage
        var dataDirectory = Environment.GetEnvironmentVariable("HOME") + "/data";
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }
        connectionString = $"Data Source={dataDirectory}/quiz.db";
    }
    else
    {
        // Local development
        connectionString = "Data Source=quiz.db";
    }
    options.UseSqlite(connectionString);
});


var app = builder.Build();
if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Database seeding with comprehensive question set
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<QuizDbContext>>();
        using var db = dbContextFactory.CreateDbContext();

        Console.WriteLine("Initializing database...");
        db.Database.EnsureCreated();

        // STEP 4: Database Schema Migration for Exam table
        try
        {
            Console.WriteLine("Checking for database schema updates...");

            // Test if new columns exist by trying to query them
            var testExam = await db.Exams.FirstOrDefaultAsync();
            if (testExam != null)
            {
                // Try to access new properties - if they don't exist, add them
                try
                {
                    var testTimeLimit = testExam.TimeLimit;
                    var testNumberOfQuestions = testExam.NumberOfQuestions;
                    Console.WriteLine("Database schema is up to date");
                }
                catch (Exception)
                {
                    Console.WriteLine("Adding new columns to Exams table...");

                    // Add new columns to existing Exams table
                    await db.Database.ExecuteSqlRawAsync(@"
                        ALTER TABLE Exams ADD COLUMN NumberOfQuestions INTEGER DEFAULT 10;
                    ");

                    await db.Database.ExecuteSqlRawAsync(@"
                        ALTER TABLE Exams ADD COLUMN TimeLimit INTEGER DEFAULT 30;
                    ");

                    await db.Database.ExecuteSqlRawAsync(@"
                        ALTER TABLE Exams ADD COLUMN AllowedQuestionTypes TEXT DEFAULT 'MultipleChoice,TrueFalse,OneWord,ShortAnswer';
                    ");

                    await db.Database.ExecuteSqlRawAsync(@"
                        ALTER TABLE Exams ADD COLUMN CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP;
                    ");

                    await db.Database.ExecuteSqlRawAsync(@"
                        ALTER TABLE Exams ADD COLUMN CreatedBy TEXT DEFAULT 'Admin';
                    ");

                    await db.Database.ExecuteSqlRawAsync(@"
                        ALTER TABLE Exams ADD COLUMN IsActive INTEGER DEFAULT 1;
                    ");

                    Console.WriteLine("Database schema updated successfully!");
                }
            }

            // Update existing exams to have default values for new fields
            var existingExams = await db.Exams.ToListAsync();
            bool needsUpdate = false;

            foreach (var existingExam in existingExams)
            {
                if (existingExam.NumberOfQuestions == 0)
                {
                    existingExam.NumberOfQuestions = 10;
                    needsUpdate = true;
                }
                if (existingExam.TimeLimit == 0)
                {
                    existingExam.TimeLimit = 30;
                    needsUpdate = true;
                }
                if (string.IsNullOrEmpty(existingExam.AllowedQuestionTypes))
                {
                    existingExam.AllowedQuestionTypes = "MultipleChoice,TrueFalse,OneWord,ShortAnswer";
                    needsUpdate = true;
                }
                if (existingExam.CreatedDate == DateTime.MinValue)
                {
                    existingExam.CreatedDate = DateTime.UtcNow;
                    needsUpdate = true;
                }
                if (string.IsNullOrEmpty(existingExam.CreatedBy))
                {
                    existingExam.CreatedBy = "Admin";
                    needsUpdate = true;
                }
            }

            if (needsUpdate)
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Updated existing exams with new field values");
            }
        }
        catch (Exception schemaEx)
        {
            Console.WriteLine($"Schema migration error: {schemaEx.Message}");
        }

        // Question seeding (your existing code)
        if (!db.Questions.Any())
        {
            Console.WriteLine("Seeding database with comprehensive question set...");

            db.Questions.AddRange(
                // MULTIPLE CHOICE QUESTIONS (MCQs)
                new Question
                {
                    QuestionText = "What keyword declares a variable in C#?",
                    Type = QuestionType.MultipleChoice,
                    Option1 = "var",
                    Option2 = "variable",
                    Option3 = "new",
                    Option4 = "declare",
                    CorrectAnswer = 1,
                    Points = 1,
                    IsActive = true,
                    DifficultyLevel = 1,
                    CreatedDate = DateTime.Now,
                    Category = "C# Basics",
                    Subject = "Programming"
                },
                new Question
                {
                    QuestionText = "Which type is NOT built-in to C#?",
                    Type = QuestionType.MultipleChoice,
                    Option1 = "int",
                    Option2 = "string",
                    Option3 = "var",
                    Option4 = "float",
                    CorrectAnswer = 3,
                    Points = 1,
                    IsActive = true,
                    DifficultyLevel = 1,
                    CreatedDate = DateTime.Now,
                    Category = "Data Types",
                    Subject = "Programming"
                },
                new Question
                {
                    QuestionText = "What is .NET?",
                    Type = QuestionType.MultipleChoice,
                    Option1 = "Operating System",
                    Option2 = "Development Framework",
                    Option3 = "Programming Language",
                    Option4 = "Database",
                    CorrectAnswer = 2,
                    Points = 1,
                    IsActive = true,
                    DifficultyLevel = 1,
                    CreatedDate = DateTime.Now,
                    Category = "Framework",
                    Subject = "Development"
                },
                new Question
                {
                    QuestionText = "Which access modifier makes a member accessible only within the same class?",
                    Type = QuestionType.MultipleChoice,
                    Option1 = "public",
                    Option2 = "private",
                    Option3 = "protected",
                    Option4 = "internal",
                    CorrectAnswer = 2,
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Access Modifiers",
                    Subject = "OOP"
                },
                new Question
                {
                    QuestionText = "What does OOP stand for?",
                    Type = QuestionType.MultipleChoice,
                    Option1 = "Object-Oriented Programming",
                    Option2 = "Open-Oriented Programming",
                    Option3 = "Optimal-Oriented Programming",
                    Option4 = "Output-Oriented Programming",
                    CorrectAnswer = 1,
                    Points = 1,
                    IsActive = true,
                    DifficultyLevel = 1,
                    CreatedDate = DateTime.Now,
                    Category = "Programming Concepts",
                    Subject = "OOP"
                },
                new Question
                {
                    QuestionText = "Which keyword is used to inherit from a class in C#?",
                    Type = QuestionType.MultipleChoice,
                    Option1 = "extends",
                    Option2 = "inherits",
                    Option3 = ":",
                    Option4 = "base",
                    CorrectAnswer = 3,
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Inheritance",
                    Subject = "OOP"
                },
                new Question
                {
                    QuestionText = "Which collection type allows duplicate values?",
                    Type = QuestionType.MultipleChoice,
                    Option1 = "HashSet",
                    Option2 = "Dictionary",
                    Option3 = "List",
                    Option4 = "SortedSet",
                    CorrectAnswer = 3,
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Collections",
                    Subject = "Data Structures"
                },
                new Question
                {
                    QuestionText = "What is the correct way to declare an array in C#?",
                    Type = QuestionType.MultipleChoice,
                    Option1 = "int[] arr = new int[5];",
                    Option2 = "int arr[] = new int[5];",
                    Option3 = "array<int> arr = new array<int>[5];",
                    Option4 = "int arr = new int[5];",
                    CorrectAnswer = 1,
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Arrays",
                    Subject = "Data Structures"
                },
                new Question
                {
                    QuestionText = "Which statement is used to handle exceptions in C#?",
                    Type = QuestionType.MultipleChoice,
                    Option1 = "try-catch",
                    Option2 = "if-else",
                    Option3 = "switch-case",
                    Option4 = "for-loop",
                    CorrectAnswer = 1,
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Exception Handling",
                    Subject = "Error Management"
                },
                new Question
                {
                    QuestionText = "Which method is used to convert a string to integer in C#?",
                    Type = QuestionType.MultipleChoice,
                    Option1 = "Convert.ToInt32()",
                    Option2 = "int.Parse()",
                    Option3 = "int.TryParse()",
                    Option4 = "All of the above",
                    CorrectAnswer = 4,
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Type Conversion",
                    Subject = "Data Types"
                },

                // TRUE/FALSE QUESTIONS
                new Question
                {
                    QuestionText = "C# is a case-sensitive language.",
                    Type = QuestionType.TrueFalse,
                    CorrectAnswer = 1,
                    Points = 1,
                    IsActive = true,
                    DifficultyLevel = 1,
                    CreatedDate = DateTime.Now,
                    Category = "Language Features",
                    Subject = "C# Basics",
                    Explanation = "C# is indeed case-sensitive, meaning 'Variable' and 'variable' are different identifiers."
                },
                new Question
                {
                    QuestionText = "Arrays in C# are zero-indexed.",
                    Type = QuestionType.TrueFalse,
                    CorrectAnswer = 1,
                    Points = 1,
                    IsActive = true,
                    DifficultyLevel = 1,
                    CreatedDate = DateTime.Now,
                    Category = "Arrays",
                    Subject = "Data Structures",
                    Explanation = "Arrays in C# start at index 0, not 1."
                },
                new Question
                {
                    QuestionText = "C# supports multiple inheritance from classes.",
                    Type = QuestionType.TrueFalse,
                    CorrectAnswer = 0,
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Inheritance",
                    Subject = "OOP",
                    Explanation = "C# supports single inheritance from classes, but multiple inheritance through interfaces."
                },
                new Question
                {
                    QuestionText = "The 'var' keyword can be used without initialization.",
                    Type = QuestionType.TrueFalse,
                    CorrectAnswer = 0,
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Variables",
                    Subject = "C# Basics",
                    Explanation = "The 'var' keyword requires immediate initialization so the compiler can infer the type."
                },
                new Question
                {
                    QuestionText = "C# is an object-oriented programming language.",
                    Type = QuestionType.TrueFalse,
                    CorrectAnswer = 1,
                    Points = 1,
                    IsActive = true,
                    DifficultyLevel = 1,
                    CreatedDate = DateTime.Now,
                    Category = "Programming Paradigms",
                    Subject = "OOP",
                    Explanation = "C# is primarily an object-oriented programming language with additional paradigm support."
                },
                new Question
                {
                    QuestionText = "String is a value type in C#.",
                    Type = QuestionType.TrueFalse,
                    CorrectAnswer = 0,
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Data Types",
                    Subject = "Memory Management",
                    Explanation = "String is a reference type in C#, stored on the heap."
                },
                new Question
                {
                    QuestionText = "Static methods can be overridden in C#.",
                    Type = QuestionType.TrueFalse,
                    CorrectAnswer = 0,
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Methods",
                    Subject = "OOP",
                    Explanation = "Static methods cannot be overridden because they belong to the type, not instances."
                },
                new Question
                {
                    QuestionText = "C# automatically manages memory through garbage collection.",
                    Type = QuestionType.TrueFalse,
                    CorrectAnswer = 1,
                    Points = 1,
                    IsActive = true,
                    DifficultyLevel = 1,
                    CreatedDate = DateTime.Now,
                    Category = "Memory Management",
                    Subject = "Runtime",
                    Explanation = "C# uses automatic garbage collection to manage memory allocation and deallocation."
                },

                // ONE WORD QUESTIONS
                new Question
                {
                    QuestionText = "What is the default value of a bool variable in C#?",
                    Type = QuestionType.OneWord,
                    CorrectTextAnswer = "false",
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Default Values",
                    Subject = "Data Types",
                    Explanation = "The default value of a bool in C# is false."
                },
                new Question
                {
                    QuestionText = "What keyword creates a new instance of a class?",
                    Type = QuestionType.OneWord,
                    CorrectTextAnswer = "new",
                    Points = 1,
                    IsActive = true,
                    DifficultyLevel = 1,
                    CreatedDate = DateTime.Now,
                    Category = "Object Creation",
                    Subject = "OOP",
                    Explanation = "The 'new' keyword is used to create new instances of classes."
                },
                new Question
                {
                    QuestionText = "What access modifier allows access from anywhere?",
                    Type = QuestionType.OneWord,
                    CorrectTextAnswer = "public",
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Access Modifiers",
                    Subject = "OOP",
                    Explanation = "The 'public' access modifier allows access from any code location."
                },
                new Question
                {
                    QuestionText = "What keyword is used to define a constant in C#?",
                    Type = QuestionType.OneWord,
                    CorrectTextAnswer = "const",
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Constants",
                    Subject = "Variables",
                    Explanation = "The 'const' keyword is used to declare compile-time constants."
                },
                new Question
                {
                    QuestionText = "What data type stores whole numbers in C#?",
                    Type = QuestionType.OneWord,
                    CorrectTextAnswer = "int",
                    Points = 1,
                    IsActive = true,
                    DifficultyLevel = 1,
                    CreatedDate = DateTime.Now,
                    Category = "Data Types",
                    Subject = "Primitives",
                    Explanation = "The 'int' data type stores 32-bit signed integers."
                },
                new Question
                {
                    QuestionText = "What keyword is used to exit a loop prematurely?",
                    Type = QuestionType.OneWord,
                    CorrectTextAnswer = "break",
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Control Flow",
                    Subject = "Loops",
                    Explanation = "The 'break' statement terminates the nearest enclosing loop."
                },
                new Question
                {
                    QuestionText = "What keyword is used to skip the current iteration of a loop?",
                    Type = QuestionType.OneWord,
                    CorrectTextAnswer = "continue",
                    Points = 2,
                    IsActive = true,
                    DifficultyLevel = 2,
                    CreatedDate = DateTime.Now,
                    Category = "Control Flow",
                    Subject = "Loops",
                    Explanation = "The 'continue' statement skips the rest of the current iteration and moves to the next."
                },

                // SHORT ANSWER QUESTIONS
                new Question
                {
                    QuestionText = "Explain the difference between 'ref' and 'out' parameters in C#.",
                    Type = QuestionType.ShortAnswer,
                    CorrectTextAnswer = "ref requires initialization before passing, out doesn't require initialization but must be assigned before method returns",
                    Points = 3,
                    IsActive = true,
                    DifficultyLevel = 3,
                    CreatedDate = DateTime.Now,
                    Category = "Parameters",
                    Subject = "Methods",
                    Explanation = "ref parameters pass by reference and must be initialized; out parameters don't need initialization but must be assigned in the method."
                },
                new Question
                {
                    QuestionText = "What is the purpose of the 'using' statement in C#?",
                    Type = QuestionType.ShortAnswer,
                    CorrectTextAnswer = "It automatically disposes resources and ensures proper cleanup",
                    Points = 3,
                    IsActive = true,
                    DifficultyLevel = 3,
                    CreatedDate = DateTime.Now,
                    Category = "Resource Management",
                    Subject = "Memory Management",
                    Explanation = "The using statement ensures automatic disposal of resources implementing IDisposable."
                },
                new Question
                {
                    QuestionText = "Describe what polymorphism means in C#.",
                    Type = QuestionType.ShortAnswer,
                    CorrectTextAnswer = "The ability of objects to take multiple forms through inheritance and interfaces",
                    Points = 3,
                    IsActive = true,
                    DifficultyLevel = 3,
                    CreatedDate = DateTime.Now,
                    Category = "Polymorphism",
                    Subject = "OOP",
                    Explanation = "Polymorphism allows objects to be treated as instances of their parent class while maintaining their specific behavior."
                },
                new Question
                {
                    QuestionText = "What is the difference between a class and a struct in C#?",
                    Type = QuestionType.ShortAnswer,
                    CorrectTextAnswer = "classes are reference types stored on heap, structs are value types stored on stack",
                    Points = 3,
                    IsActive = true,
                    DifficultyLevel = 3,
                    CreatedDate = DateTime.Now,
                    Category = "Types",
                    Subject = "Memory Management",
                    Explanation = "Classes are reference types with heap allocation; structs are value types with stack allocation."
                },
                new Question
                {
                    QuestionText = "Explain the concept of encapsulation in object-oriented programming.",
                    Type = QuestionType.ShortAnswer,
                    CorrectTextAnswer = "Encapsulation is bundling data and methods together while hiding internal implementation details",
                    Points = 3,
                    IsActive = true,
                    DifficultyLevel = 3,
                    CreatedDate = DateTime.Now,
                    Category = "Encapsulation",
                    Subject = "OOP",
                    Explanation = "Encapsulation protects data integrity by controlling access through public interfaces."
                },
                new Question
                {
                    QuestionText = "What is the difference between abstract classes and interfaces in C#?",
                    Type = QuestionType.ShortAnswer,
                    CorrectTextAnswer = "Abstract classes can have implementation and constructors, interfaces only define contracts",
                    Points = 3,
                    IsActive = true,
                    DifficultyLevel = 3,
                    CreatedDate = DateTime.Now,
                    Category = "Abstraction",
                    Subject = "OOP",
                    Explanation = "Abstract classes provide partial implementation; interfaces define pure contracts for implementation."
                }
            );

            db.SaveChanges();
            Console.WriteLine("Database seeding completed successfully!");
            Console.WriteLine("Added comprehensive question set:");
            Console.WriteLine("- 10 Multiple Choice Questions");
            Console.WriteLine("- 8 True/False Questions");
            Console.WriteLine("- 7 One Word Questions");
            Console.WriteLine("- 6 Short Answer Questions");
            Console.WriteLine("Total: 31 questions across all categories");
        }
        else
        {
            Console.WriteLine("Database already contains questions - skipping seeding");
        }
    }
}
catch (Exception dbEx)
{
    Console.WriteLine($"Database initialization failed: {dbEx.Message}");
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

Console.WriteLine("Starting Blazor Quiz Application...");
app.Run();


