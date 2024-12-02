namespace Jumpeno.Shared.Utils;

/// <summary>
///     Utilizes functionalities for discrete and continuous collision detection.
///     
///     If continuous collision is detected, method returns true and output parameters are:
///     1) point - point where collision was detected
///     2) normal - a normal vector pointing to direction in which the collision must be resolved
///     3) t - argument in parametric equation of given vector where collision was detected
///     
///     Resolution methods return true and call onResolve delegate if any collision was detected,
///     delegate provides parameters:
///     1) Resolved - resolved position
///     2) Normal - a normal vector pointing to direction in which the collision was resolved
/// </summary>
public static class Collision {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int PRECISION = 3;
    public static readonly float UNIT = MathF.Pow(10, -PRECISION);
    public static readonly PointF ZERO_VECTOR = new(0, 0);

    // Validation -------------------------------------------------------------------------------------------------------------------------
    public static bool ValidateBoundary(RectangleF boundary, RectFPosition position) {
        return boundary.Width >= position.Width && boundary.Height >= position.Height;
    }
    public static void CheckBoundary(RectangleF boundary, RectFPosition position) {
        if (ValidateBoundary(boundary, position)) return;
        throw new ArgumentException("Object does not fit into boundary!");
    }

    public static bool ValidateRectDimensions(RectangleF rect) {
        return rect.Width > 0 && rect.Height > 0;
    }
    public static void CheckRectDimensions(RectangleF rect) {
        if (ValidateRectDimensions(rect)) return;
        throw new ArgumentException("Invalid rectangle dimensions!");
    }
    
    public static bool ValidateRectMoveDimensions(RectangleF before, RectangleF after) {
        return before.Width == after.Width && before.Height == after.Height;
    }
    public static void CheckRectMoveDimensions(RectangleF before, RectangleF after) {
        if (ValidateRectMoveDimensions(before, after)) return;
        throw new ArgumentException("Moving rectangle dimensions changed!");
    }

    // Discrete collisions ----------------------------------------------------------------------------------------------------------------
    public static bool RectVSPoint(RectangleF rect, PointF point) {
        return rect.X <= point.X && point.X <= rect.X + rect.Width
               && rect.Y <= point.Y && point.Y <= rect.Y + rect.Height; 
    }

    public static bool RectVSLine(RectangleF rect, (PointF Point1, PointF Point2) vector) {
        return RectVSLine(rect, vector, out var cp, out var cn, out var t);
    }

    public static bool RectVSRect(RectangleF rect1, RectangleF rect2) {
        return rect1.IntersectsWith(rect2);
    }

    // Continuous collisions --------------------------------------------------------------------------------------------------------------
    public static bool RectVSRay(
        RectangleF rect, (PointF Point1, PointF Point2) vector,
        out PointF? point, out PointF? normal, out float? t
    ) {
        // 1) Initialization:
        // 1.1) Input:
        var start = vector.Point1;
        var end = vector.Point2;
        // 1.2) Output:
        point = null;
        normal = null;
        t = null;

        // 2) Computation:
        // 2.1) Ray direction:
        var rayDir = new PointF(end.X - start.X, end.Y - start.Y);
        if ((rect.Width == 0 && rect.Height == 0) || (rayDir.X == 0 && rayDir.Y == 0)) return false;
        // 2.2) Near & far collision time:
        var tNearX = rayDir.X == 0 ? start.X < rect.X ? float.MaxValue : float.MinValue : (rect.X - start.X) / rayDir.X;
        var tNearY = rayDir.Y == 0 ? start.Y < rect.Y ? float.MaxValue : float.MinValue : (rect.Y - start.Y) / rayDir.Y;
        var tFarX =  rayDir.X == 0 ? start.X <= rect.X + rect.Width ? float.MaxValue : float.MinValue : (rect.X + rect.Width - start.X) / rayDir.X;
        var tFarY = rayDir.Y == 0 ? start.Y <= rect.Y + rect.Height ? float.MaxValue : float.MinValue : (rect.Y + rect.Height - start.Y) / rayDir.Y;
        // 2.3) Swap nearest times:
        if (tNearX > tFarX) (tNearX, tFarX) = (tFarX, tNearX);
        if (tNearY > tFarY) (tNearY, tFarY) = (tFarY, tNearY);
        if (tNearX > tFarY || tNearY > tFarX) return false;
        // 2.4) Calculate result:
        var tHitNear = Math.Max(tNearX, tNearY);
        var tHitFar = Math.Min(tFarX, tFarY);
        if (tHitFar < 0) return false;

        // 3) Output:
        // 3.1) Contact point:
        point = new PointF(
            start.X + tHitNear * rayDir.X,
            start.Y + tHitNear * rayDir.Y
        );
        // 3.2) Contact normal:
        if (tNearX > tNearY) {
            if (rayDir.X < 0) normal = new PointF(1, 0);
            else normal = new PointF(-1, 0);
        } else {
            if (rayDir.Y < 0) normal = new PointF(0, 1);
            else normal = new PointF(0, -1);
        }
        // 3.3) Hit time:
        t = tHitNear;
        // 3.4) Result:
        return true;
    }

    public static bool RectVSLine(
        RectangleF rect, (PointF Point1, PointF Point2) vector,
        out PointF? point, out PointF? normal, out float? t
    ) {
        // 1) Check ray collision:
        var result = RectVSRay(rect, vector, out point, out normal, out t);
        // 2) Apply only for line (parameter t = <0, 1>):
        return t != null && t <= 1.0 && result;
    }

    public static bool RectVSRectMove(
        RectangleF rect, RectangleF before, RectangleF after,
        out PointF? point, out PointF? normal, out float? t
    ) {
        // 1) Check valid parameters:
        CheckRectDimensions(before);
        CheckRectMoveDimensions(before, after);
        // 2) Compute boundary:
        var boundary = new RectangleF(
            rect.X - before.Width / 2 + UNIT,
            rect.Y - before.Height / 2 + UNIT,
            rect.Width + before.Width - 2 * UNIT,
            rect.Height + before.Height - 2 * UNIT
        );
        // 3) Check vector collision with boundary:
        return RectVSLine(
            boundary, new(
                new(before.X + before.Width / 2, before.Y + before.Height / 2),
                new(after.X + after.Width / 2, after.Y + after.Height / 2)
            ), out point, out normal, out t
        );
    }
    
    // Discrete collision resolution ------------------------------------------------------------------------------------------------------
    public static bool Resolve(
        float level, POSITION deny, RectFPosition position,
        Action<(RectFPosition Resolved, PointF Normal)> onResolve
    ) {
        // 1) Define variables:
        var halfWidth = position.Width / 2;
        var halfHeight = position.Height / 2;
        var normal = new PointF(0, 0);
        // 2) Check collision:
        switch (deny) {
            case POSITION.LEFT: if (position.Center.X - halfWidth < level) normal.X = 1; break;
            case POSITION.RIGHT: if (position.Center.X + halfWidth > level) normal.X = -1; break;
            case POSITION.BOTTOM: if (position.Center.Y - halfHeight < level) normal.Y = 1; break;
            case POSITION.TOP: if (position.Center.Y + halfHeight > level) normal.Y = -1; break;
        }
        // 3) Resolve collision:
        if (normal.Equals(ZERO_VECTOR)) return false;
        if (normal.X != 0) position.Center.X = level + normal.X * halfWidth;
        if (normal.Y != 0) position.Center.Y = level + normal.Y * halfHeight;
        // 4) Apply result:
        onResolve((position, normal)); return true;
    }

    public static bool Resolve(
        RectangleF boundary, RectFPosition position,
        Action<(RectFPosition Resolved, PointF Normal)> onResolve
    ) {
        // 1) Check arguments:
        CheckBoundary(boundary, position);
        // 2) Define variables:
        var normal = new PointF(0, 0);
        void resolve((RectFPosition resolved, PointF normal) fix) {
            position = fix.resolved;
            normal.X = fix.normal.X;
            normal.Y = fix.normal.Y;
        };
        // 3) Resolve collisions:
        if (!Resolve(boundary.X, POSITION.LEFT, position, resolve))
            Resolve(boundary.X + boundary.Width, POSITION.RIGHT, position, resolve);
        if (!Resolve(boundary.Y, POSITION.BOTTOM, position, resolve))
            Resolve(boundary.Y + boundary.Height, POSITION.TOP, position, resolve);
        // 4) Apply result:
        if (normal.Equals(ZERO_VECTOR)) return false;
        onResolve((position, normal)); return true;
    }

    // Continuous collision resolution ----------------------------------------------------------------------------------------------------
    public static bool Resolve<T>(
        List<T> items, RectFPosition before, RectFPosition after,
        Action<(RectFPosition Resolved, PointF Normal)> onResolve
    ) where T : IRectFPositionable {
        // 1) Define variables:
        var collision = false;
        // 2) Check and order colliding items (to prevent neighbor blocks):
        PriorityQueue<(IRectFPositionable Item, PointF Normal), double> collisions = new();
        foreach (var item in items) {
            if (!RectVSRectMove(item.Rect, GetBoundingBox(before), GetBoundingBox(after), out var point, out var normal, out var t)) continue;
            collisions.Enqueue((item, (PointF) normal!), DistanceSquared(item.Position.Center, (PointF) point!));
        }
        // 3) Resolve collisions (in correct order):
        while (collisions.Count > 0) {
            // 3.1) Check if collision still exists:
            var (item, normal) = collisions.Dequeue();
            var moveBox = GetMoveBox(before, after);
            if (!RectVSRect(item.Rect, moveBox)) continue;
            // 3.2) Resolve collision:
            if (normal.X < 0) after.Center.X = item.Position.Center.X - item.Position.Width / 2 - after.Width / 2;
            else if (normal.X > 0) after.Center.X = item.Position.Center.X + item.Position.Width / 2 + after.Width / 2;
            else if (normal.Y < 0) after.Center.Y = item.Position.Center.Y - item.Position.Height / 2 - after.Height / 2;
            else if (normal.Y > 0) after.Center.Y = item.Position.Center.Y + item.Position.Height / 2 + after.Height / 2;
            onResolve((after, normal));
            collision = true;
        }
        // 4) Return result:
        return collision;
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static double DistanceSquared(PointF point1, PointF point2) {
        return MathF.Pow(point1.X - point2.X, 2) + MathF.Pow(point1.Y - point2.Y, 2);
    }
    public static double Distance(PointF point1, PointF point2) {
        return Math.Sqrt(DistanceSquared(point1, point2));
    }

    public static RectangleF GetBoundingBox(RectFPosition position) {
        return new(
            position.Center.X - position.Width / 2,
            position.Center.Y - position.Height / 2,
            position.Width,
            position.Height
        );
    }
    
    public static RectangleF GetMoveBox(RectangleF before, RectangleF after) {
        var startX = Math.Min(before.X, after.X);
        var startY = Math.Min(before.Y, after.Y);
        var endX = Math.Max(before.X + before.Width, after.X + after.Width);
        var endY = Math.Max(before.Y + before.Height, after.Y + after.Height);
        return new RectangleF(startX, startY, endX - startX, endY - startY);
    }
    public static RectangleF GetMoveBox(RectFPosition before, RectFPosition after) {
        return GetMoveBox(GetBoundingBox(before), GetBoundingBox(after));
    }
}
