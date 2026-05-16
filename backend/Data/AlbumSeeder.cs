using backend.Entities;
using backend.Extensions;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>Seeds Vietnamese album tree and demo files.</summary>
public static class AlbumSeeder
{
    /// <summary>Seeds album hierarchy. Skips if any album already exists.</summary>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.Albums.AnyAsync()) return;

        var now = DateTimeOffset.UtcNow;

        // ── Albums gốc ───────────────────────────────────────────────────────
        var nhiepAnh = MakeAlbum("nhiep-anh", "Nhiếp ảnh",  "Bộ sưu tập các tác phẩm nhiếp ảnh.",        null, 1, now);
        var blog     = MakeAlbum("blog",      "Blog",        "Hình ảnh và tài sản dùng trong blog.",       null, 2, now);
        var duAn     = MakeAlbum("du-an",     "Dự án",       "Ảnh chụp màn hình và tài sản dự án.",        null, 3, now);

        // ── Con của Nhiếp ảnh ────────────────────────────────────────────────
        var duLich   = MakeAlbum("du-lich",    "Du lịch",    "Ảnh chụp trong những chuyến đi.",            nhiepAnh.Id, 1, now);
        var thienNhien = MakeAlbum("thien-nhien","Thiên nhiên","Phong cảnh, thực vật và động vật hoang dã.", nhiepAnh.Id, 2, now);
        var thoiTrang  = MakeAlbum("thoi-trang","Thời trang", "Nhiếp ảnh thời trang và chân dung.",         nhiepAnh.Id, 3, now);
        var duongPho   = MakeAlbum("duong-pho", "Đường phố", "Nhiếp ảnh đường phố đô thị.",                nhiepAnh.Id, 4, now);

        // ── Con của Du lịch ──────────────────────────────────────────────────
        var nhatBan  = MakeAlbum("nhat-ban",   "Nhật Bản",   "Đất nước mặt trời mọc.",                    duLich.Id, 1, now);
        var vietNam  = MakeAlbum("viet-nam",   "Việt Nam",   "Vẻ đẹp dải đất hình chữ S.",                duLich.Id, 2, now);
        var chauAu   = MakeAlbum("chau-au",    "Châu Âu",    "Những chuyến phiêu lưu châu Âu.",           duLich.Id, 3, now);

        // ── Con của Thiên nhiên ──────────────────────────────────────────────
        var rung  = MakeAlbum("rung",  "Rừng", "Sâu trong lòng đại ngàn.",                                thienNhien.Id, 1, now);
        var bien  = MakeAlbum("bien",  "Biển", "Dưới và bên cạnh đại dương.",                             thienNhien.Id, 2, now);

        // ── Con của Blog ─────────────────────────────────────────────────────
        var anhBia   = MakeAlbum("anh-bia",  "Ảnh bìa",  "Ảnh bìa cho các bài viết.",                    blog.Id, 1, now);
        var banner   = MakeAlbum("banner",   "Banner",   "Banner cho trang chủ.",                         blog.Id, 2, now);

        db.Albums.AddRange(
            nhiepAnh, blog, duAn,
            duLich, thienNhien, thoiTrang, duongPho,
            nhatBan, vietNam, chauAu,
            rung, bien,
            anhBia, banner
        );

        // ── Tệp demo ─────────────────────────────────────────────────────────
        var files = new[]
        {
            // Nhật Bản
            MakeFile("thap-tokyo",        "Tháp Tokyo",          "https://images.unsplash.com/photo-1540959733332-eab4deabeeaf?w=800"),
            MakeFile("giao-lo-shibuya",   "Giao lộ Shibuya",    "https://images.unsplash.com/photo-1542051841857-5f90071e7989?w=800"),
            MakeFile("nui-fuji",          "Núi Phú Sĩ",         "https://images.unsplash.com/photo-1578637387939-43c525550085?w=800"),

            // Việt Nam
            MakeFile("vinh-ha-long",      "Vịnh Hạ Long",       "https://images.unsplash.com/photo-1528127269322-539801943592?w=800"),
            MakeFile("den-long-hoi-an",   "Đèn lồng Hội An",    "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800"),

            // Châu Âu
            MakeFile("thap-eiffel",       "Tháp Eiffel",         "https://images.unsplash.com/photo-1511739001486-6bfe10ce785f?w=800"),
            MakeFile("santorini",         "Santorini",           "https://images.unsplash.com/photo-1469796466635-455ede028aca?w=800"),

            // Rừng
            MakeFile("rung-suong-mu",     "Rừng sương mù",      "https://images.unsplash.com/photo-1448375240586-882707db888b?w=800"),
            MakeFile("cay-nang-chieu",    "Cây nắng chiều",      "https://images.unsplash.com/photo-1426604966848-d7adac402bff?w=800"),

            // Biển
            MakeFile("bien-hoang-hon",    "Biển hoàng hôn",      "https://images.unsplash.com/photo-1505118380757-91f5f5632de0?w=800"),
            MakeFile("ran-san-ho",        "Rạn san hô",          "https://images.unsplash.com/photo-1518020382113-a7e8fc38eac9?w=800"),

            // Thời trang
            MakeFile("chan-dung-thoi-trang","Chân dung thời trang","https://images.unsplash.com/photo-1529626455594-4ff0802cfb7e?w=800"),
            MakeFile("phong-cach-duong-pho","Phong cách đường phố","https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?w=800"),

            // Đường phố
            MakeFile("anh-sang-do-thi",   "Ánh sáng đô thị",    "https://images.unsplash.com/photo-1477959858617-67f85cf4f1df?w=800"),
            MakeFile("hinh-hoc-do-thi",   "Hình học đô thị",    "https://images.unsplash.com/photo-1486325212027-8081e485255e?w=800"),

            // Ảnh bìa
            MakeFile("anh-bia-1",         "Ảnh bìa 1",           "https://images.unsplash.com/photo-1499750310107-5fef28a66643?w=800"),
            MakeFile("anh-bia-2",         "Ảnh bìa 2",           "https://images.unsplash.com/photo-1432821596592-e2c18b78144f?w=800"),

            // Banner
            MakeFile("banner-1",          "Banner 1",             "https://images.unsplash.com/photo-1493612276216-ee3925520721?w=800"),
            MakeFile("banner-2",          "Banner 2",             "https://images.unsplash.com/photo-1518655048521-f130df041f66?w=800"),
        };

        db.files.AddRange(files);

        // ── AlbumFile junctions ───────────────────────────────────────────────
        var map = new Dictionary<Album, backend.Entities.File[]>
        {
            [nhatBan]   = [files[0],  files[1],  files[2]],
            [vietNam]   = [files[3],  files[4]],
            [chauAu]    = [files[5],  files[6]],
            [rung]      = [files[7],  files[8]],
            [bien]      = [files[9],  files[10]],
            [thoiTrang] = [files[11], files[12]],
            [duongPho]  = [files[13], files[14]],
            [anhBia]    = [files[15], files[16]],
            [banner]    = [files[17], files[18]],
        };

        foreach (var (album, albumFiles) in map)
        {
            foreach (var file in albumFiles)
                db.AlbumFiles.Add(new AlbumFile { AlbumId = album.Id, FileId = file.Id });
            album.CountImageRef     = albumFiles.Length;
            album.FileDescriptionId = albumFiles[0].Id;
        }

        duLich.FileDescriptionId      = files[0].Id;
        thienNhien.FileDescriptionId  = files[7].Id;
        nhiepAnh.FileDescriptionId    = files[0].Id;
        blog.FileDescriptionId        = files[15].Id;

        await db.SaveChangesAsync();
    }

    private static Album MakeAlbum(string name, string title, string description, string? parentId, int order, DateTimeOffset now) => new()
    {
        Name        = name,
        Title       = title,
        Description = description,
        Slug        = name.ToSlug(),
        AlbumId     = parentId,
        OrderIndex  = order,
        CreatedDate = now,
        UpdatedDate = now,
    };

    private static backend.Entities.File MakeFile(string name, string title, string url) => new()
    {
        Name        = name,
        Title       = title,
        Url         = url,
        Description = string.Empty,
    };
}
