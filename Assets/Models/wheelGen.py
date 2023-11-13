# Python wheel generator
# Author: Pablo Banzo - A01782031
# Description: This script generates a 3D model of a wheel with a specified
# number of spokes, radius, and thickness. The model is exported to an OBJ file 
# format suitable for use in 3D graphics applications like Unity.

import math
import argparse # instead of sys (does not simplify the assignment)

class Point:
    def __init__(self, x=0, y=0, z=0):
        # Using Unity's coordinate system (x: right, y: up, z: forward)
        self.x = x
        self.y = y 
        self.z = z

class Vector:
    def __init__(self, point1, point2):
        self.array = [point2.x - point1.x, point2.y - point1.y, point2.z - point1.z]

    def dot(self, other):
        # Returns the dot product of this vector and another vector
        # Using zip for tuples
        return sum(a * b for a, b in zip(self.array, other.array))
    
    def cross(self, other):
        # Returns the cross product of this vector and another vector
        x = self.array[1] * other.array[2] - self.array[2] * other.array[1]
        y = self.array[2] * other.array[0] - self.array[0] * other.array[2]
        z = self.array[0] * other.array[1] - self.array[1] * other.array[0]
        return Vector(Point(0, 0, 0), Point(x, y, z))
    
    def norm(self):
        # Returns the norm of this vector (length)
        return math.sqrt(sum(map(lambda x: x ** 2, self.array)))
    
    def normalize(self):
        # Returns a normalized version of this vector
        norm_value = self.norm()
        return list(map(lambda x: x / norm_value if norm_value != 0 else 0, self.array))
    
class Face:
    def __init__(self, point_indices, normal_vector):
        self.point_indices = point_indices  # Tuple of 3 indices (triangle) in clockwise order
        self.normal_vector = normal_vector  # Vector object pointing outwards from the face

class Wheel:
    def __init__(self, spokes, radius, thickness):
        self.spokes = spokes
        self.radius = radius
        self.thickness = thickness
        self.points = []   # Will hold Point objects
        self.faces = []    # Will hold Face objects
        self.normals = []  # Will hold normal vectors

    def calculate_normal(self, p1, p2, p3):
        # Create vectors from the points of a triangle
        u = Vector(p1, p2)
        v = Vector(p1, p3)
        normal = u.cross(v)  # To follow hand rule (u x v = normal vector)
        return normal.normalize() # Normalize the vector to get a unit vector
    
    def generate(self):
        # Center points for the top and bottom (for the rim faces)
        center_top = Point(self.thickness / 2, 0, 0)
        center_bottom = Point(-self.thickness / 2, 0, 0)
        self.points.append(center_top)
        self.points.append(center_bottom)

        # Generate the points on the circumference of the wheel
        for i in range(self.spokes):
            angle_rad = 2 * math.pi * i / self.spokes
            x = self.radius * math.cos(angle_rad)
            y = self.radius * math.sin(angle_rad)
            self.points.append(Point(self.thickness / 2, y, x))
            self.points.append(Point(-self.thickness / 2, y, x))

        # Generate the side faces (connecting the top and bottom rims)
        for i in range(self.spokes):
            j = (i + 1) % self.spokes
            # Side faces (clockwise order for Unity)
            a = i * 2 + 2
            b1 = j * 2 + 3
            b2 = i * 2 + 3
            c1 = j * 2 + 2
            c2 = j * 2 + 3
            self.faces.append(Face((c1,b1,a), None))  # Face 1
            self.faces.append(Face((c2, b2, a), None))  # Face 2

        # Generate the top and bottom rim faces
        for i in range(self.spokes):
            j = (i + 1) % self.spokes
            # Top rim faces
            self.faces.append(Face((j * 2 + 2,i * 2 + 2,0), None))  # Triangle 1
            # Bottom rim faces
            self.faces.append(Face((i * 2 + 3,j * 2 + 3,1), None)) # Triangle 2

        # Calculate normals for all faces
        for face in self.faces:
            p1 = self.points[face.point_indices[0]]
            p2 = self.points[face.point_indices[1]]
            p3 = self.points[face.point_indices[2]]
            # Calculate normal for the current face
            normal = self.calculate_normal(p1, p2, p3)
            # Store the normal for the face
            face.normal_vector = normal
            self.normals.append(normal)

    # Export the wheel to an OBJ file format
    def save(self, filename):
        with open(filename, 'w') as file:
            file.write('# Wheel OBJ file\n')
            file.write(f'o Wheel_N{self.spokes}_R{self.radius}_T{self.thickness}\n')
            # Write points
            file.write(f'# Vertices: {len(self.points)}\n')
            for p in self.points:
                file.write(f'v {p.x} {p.y} {p.z}\n')
            # Write normals (one per face)
            file.write(f'# Normals: {len(self.normals)}\n')
            for i, face in enumerate(self.faces):
                normal = self.normals[i]
                file.write(f'vn {normal[0]} {normal[1]} {normal[2]}\n')
            # Write faces
            file.write(f'# Faces: {len(self.faces)}\n')
            for i, face in enumerate(self.faces):
                # OBJ format faces are 1-indexed (not 0-indexed like Python)
                file.write(f'f {face.point_indices[0]+1}//{i+1} {face.point_indices[1]+1}//{i+1} {face.point_indices[2]+1}//{i+1}\n')

# Custom type checking functions for argparse
def check_spokes(value):
    min_value = 3   # Minimum number of spokes
    max_value = 360 # Maximum number of spokes
    ivalue = int(value)
    if ivalue < min_value or ivalue > max_value:
        raise argparse.ArgumentTypeError(f"Number of spokes must be between {min_value} and {max_value}")
    return ivalue

def positive_float(value):
    fvalue = float(value)
    if fvalue <= 0:
        raise argparse.ArgumentTypeError(f"{value} is an invalid positive float value")
    return fvalue

def main():
    # Using argparse instead of sys.argv for better documentation
    parser = argparse.ArgumentParser(
                    prog='WheelMaker',
                    description='This program will generate a wheel obj file based on the parameters provided',
                    epilog='Created by: Pablo Banzo')
    parser.add_argument('-n', '--spokes', type=check_spokes, default=8, help='Number of spokes between 3 and 360')
    parser.add_argument('-r', '--radius', type=positive_float, default=1.0, help='Radius of the wheel (positive float)')
    parser.add_argument('-t', '--thickness', type=positive_float, default=0.5, help='Thickness of the wheel (positive float)')
    
    args = parser.parse_args()

    wheel = Wheel(args.spokes, args.radius, args.thickness)
    wheel.generate()
    wheel.save('wheel.obj')

if __name__ == "__main__":
    main()