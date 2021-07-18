using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace LandOfRails_Discord_Bot_DOTNET.Models
{
    public partial class lordiscordbotContext : DbContext
    {
        public lordiscordbotContext()
        {
        }

        public virtual DbSet<CommandIdea> CommandIdeas { get; set; }
        public virtual DbSet<ElectionLineup> ElectionLineups { get; set; }
        public virtual DbSet<Launcher> Launchers { get; set; }
        public virtual DbSet<LauncherAccess> LauncherAccesses { get; set; }
        public virtual DbSet<Poll> Polls { get; set; }
        public virtual DbSet<PollOption> PollOptions { get; set; }
        public virtual DbSet<PollVotedRegister> PollVotedRegisters { get; set; }
        public virtual DbSet<TeamRulesAccepted> TeamRulesAccepteds { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySQL("server=landofrails.net;uid=lor-discord-bot;pwd=v3iLYCTPLvaLPC9;database=lor-discord-bot");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommandIdea>(entity =>
            {
                entity.ToTable("commandIdeas");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.Command).IsRequired();

                entity.Property(e => e.Count).HasColumnType("smallint(6)");

                entity.Property(e => e.JumpUrl).IsRequired();

                entity.Property(e => e.MemberId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("MemberID");

                entity.Property(e => e.MemberUsername).IsRequired();
            });

            modelBuilder.Entity<ElectionLineup>(entity =>
            {
                entity.ToTable("ElectionLineup");

                entity.HasIndex(e => e.ByMemberId, "ByMemberID");

                entity.HasIndex(e => new { e.MemberId, e.ByMemberId }, "MemberID");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.ByMemberId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("ByMemberID");

                entity.Property(e => e.MemberId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("MemberID");

                entity.HasOne(d => d.ByMember)
                    .WithMany(p => p.ElectionLineupByMembers)
                    .HasForeignKey(d => d.ByMemberId)
                    .HasConstraintName("ElectionLineup_ibfk_2");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.ElectionLineupMembers)
                    .HasForeignKey(d => d.MemberId)
                    .HasConstraintName("ElectionLineup_ibfk_1");
            });

            modelBuilder.Entity<Launcher>(entity =>
            {
                entity.HasKey(e => e.FkMemberId)
                    .HasName("PRIMARY");

                entity.ToTable("launcher");

                entity.Property(e => e.FkMemberId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("FK_Member_ID");

                entity.Property(e => e.ModpackShortcut)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.HasOne(d => d.FkMember)
                    .WithOne(p => p.Launcher)
                    .HasForeignKey<Launcher>(d => d.FkMemberId)
                    .HasConstraintName("launcher_ibfk_1");
            });

            modelBuilder.Entity<LauncherAccess>(entity =>
            {
                entity.ToTable("launcherAccess");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.MemberId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("MemberID");

                entity.Property(e => e.ModpackShortcut)
                    .IsRequired()
                    .HasMaxLength(64);
            });

            modelBuilder.Entity<Poll>(entity =>
            {
                entity.HasIndex(e => e.MemberId, "MemberID");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.EndDatetime).HasColumnName("End_Datetime");

                entity.Property(e => e.MemberId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("MemberID")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MessageId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("MessageID");

                entity.Property(e => e.Question).IsRequired();

                entity.Property(e => e.StartDatetime).HasColumnName("Start_datetime");

                entity.Property(e => e.TextChannelId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("TextChannelID");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.Polls)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("Polls_ibfk_1");
            });

            modelBuilder.Entity<PollOption>(entity =>
            {
                entity.HasIndex(e => e.FkPollId, "FK_Poll_ID");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.EmojiUnicode)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.FkPollId)
                    .HasColumnType("int(11)")
                    .HasColumnName("FK_Poll_ID");

                entity.Property(e => e.VoteOption).IsRequired();

                entity.Property(e => e.Votes).HasColumnType("int(11)");

                entity.HasOne(d => d.FkPoll)
                    .WithMany(p => p.PollOptions)
                    .HasForeignKey(d => d.FkPollId)
                    .HasConstraintName("PollOptions_ibfk_1");
            });

            modelBuilder.Entity<PollVotedRegister>(entity =>
            {
                entity.ToTable("PollVotedRegister");

                entity.HasIndex(e => e.FkPollOptionsId, "FK_PollOptions_ID");

                entity.HasIndex(e => e.FkMemberId, "FK_Poll_ID");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.FkMemberId)
                    .HasColumnType("bigint(11)")
                    .HasColumnName("FK_Member_ID");

                entity.Property(e => e.FkPollOptionsId)
                    .HasColumnType("int(11)")
                    .HasColumnName("FK_PollOptions_ID");

                entity.HasOne(d => d.FkMember)
                    .WithMany(p => p.PollVotedRegisters)
                    .HasForeignKey(d => d.FkMemberId)
                    .HasConstraintName("PollVotedRegister_ibfk_3");

                entity.HasOne(d => d.FkPollOptions)
                    .WithMany(p => p.PollVotedRegisters)
                    .HasForeignKey(d => d.FkPollOptionsId)
                    .HasConstraintName("PollVotedRegister_ibfk_1");
            });

            modelBuilder.Entity<TeamRulesAccepted>(entity =>
            {
                entity.ToTable("teamRulesAccepted");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("ID");

                entity.Property(e => e.MemberId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("MemberID");

                entity.Property(e => e.MemberUsername).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.MemberId)
                    .HasName("PRIMARY");

                entity.Property(e => e.MemberId)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("MemberID");

                entity.Property(e => e.DiscordName).IsRequired();

                entity.Property(e => e.InteractionCount).HasColumnType("int(11)");

                entity.Property(e => e.MessageCount).HasColumnType("int(11)");

                entity.Property(e => e.ReactionCount).HasColumnType("int(11)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
