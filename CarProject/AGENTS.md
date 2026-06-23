# Agents Summary

## Goal
- Build luxury car showroom website with full UI/UX (index, listing, detail, login, deposit) using ASP.NET Core MVC + Bootstrap 5.

## Constraints & Preferences
- Premium luxury design (Mercedes-Benz, BMW, Audi, VinFast style)
- Colors: #111 (black), #FFF (white), #E5E5E5 (gray), #003366 (navy), red accent
- Font: Inter + Playfair Display
- Must include hero section, brand showcase, car cards, testimonials, floating contact buttons
- Must include car listing page with filter sidebar (brand, body type, search)
- Must include car detail page with gallery, specs, versions, CTAs
- Must include login page and deposit form
- Responsive: desktop/tablet/mobile
- Smooth page transitions, fixed navbar
- Dark hero section, light content sections

## Progress
### Done
- Rewrote `wwwroot/css/site.css` with luxury automotive theme (CSS variables, Inter + Playfair Display fonts, animations, premium section components, glassmorphism navbar, floating contact buttons, car cards with hover effects, filter sidebar, specs table, testimonials, responsive breakpoints)
- Rewrote `Views/Shared/_Layout.cshtml` with premium navbar (brand icon, brand-sub "Premium Showroom", uppercase nav links, outline login button), page-transition wrapper, floating contact buttons (Messenger/Zalo/Phone), dark footer with social links
- Rewrote `Pages/Index.cshtml` luxury homepage: hero-fullscreen with overlay/stats/badge, brand grid with gradient circles, car grid with premium cards (dark image bg, badge, car emoji), testimonials section (3 cards with stars), "Tại sao chọn chúng tôi" section
- Created `Pages/Cars.cshtml.cs` with filter query logic (search, brand filter, body type filter, sort)
- Created `Pages/Cars.cshtml` with left filter sidebar (radio buttons for brand/body type), search box, sort dropdown, car card grid with breadcrumb
- Updated `Pages/Details.cshtml.cs` to include HangXe navigation property
- Rewrote `Pages/Details.cshtml` with gallery section (breadcrumb overlay, car-display icon), info panel (brand badge, version tabs, price hero with stock status, quick specs grid, CTA group), quick specs sidebar, technical specs table
- Rewrote `Pages/Account/Login.cshtml` with premium card layout, accent button, demo credentials
- Rewrote `Pages/Orders/DepositForm.cshtml` with 2-column layout (car info card + form card), breadcrumb
- Updated `Pages/Index.cshtml.cs` to remove BannerList (simplification)
- Removed stale `Controllers/AccountController.cs` (moved to Razor Pages)

### In Progress
- (none)

### Blocked
- (none)

## Key Decisions
- Use emoji + Bootstrap Icons for car placeholders (no real car images available)
- Separate Cars listing page from Index homepage (instead of one page for both)
- Filter sidebar uses radio buttons + form submit for simplicity (no JavaScript filtering)
- Floating contact buttons fixed to bottom-right across all pages
- Dark theme hero, light theme content sections to match luxury automotive sites
- Use Razor Pages instead of MVC Controllers for page-based routes
- Remove stale Controller files in favor of Razor Pages

## Next Steps
- (none — all major redesign files are complete)

## Critical Context
- No actual car images exist in wwwroot/images; using emoji as placeholder
- Seed data in DbInitializer.cs has 5 brands, 8 car models, 10 versions with placeholder image paths
- The Cars listing page expects `Pages/Cars.cshtml` and `Pages/Cars.cshtml.cs` to exist (already created)
- Build succeeded with 0 errors
- Run app with `dotnet run --project CarProject\CarProject.csproj`

## Relevant Files
- `CarProject/wwwroot/css/site.css`: complete luxury theme (all CSS)
- `CarProject/Views/Shared/_Layout.cshtml`: premium layout (navbar + footer + floating buttons)
- `CarProject/Pages/_ViewStart.cshtml`: layout directive for Razor Pages
- `CarProject/Pages/_ViewImports.cshtml`: tag helpers and usings
- `CarProject/Pages/Index.cshtml + .cs`: luxury homepage (hero, brands, featured cars, testimonials)
- `CarProject/Pages/Cars.cshtml + .cs`: car listing with filter sidebar
- `CarProject/Pages/Details.cshtml + .cs`: car detail with specs and CTAs
- `CarProject/Pages/Account/Login.cshtml`: premium login form
- `CarProject/Pages/Orders/DepositForm.cshtml`: deposit form with 2-column layout
- `CarProject/Models/*`: EF Core models (HangXe, DongXe, PhienBanXe, etc.)
- `CarProject/Data/AppDbContext.cs`: database context
- `CarProject/Data/DbInitializer.cs`: seed data
