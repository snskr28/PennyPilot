export interface LoginRequest{
    identifier: string;
    password: string;
}

export interface SignupRequest{
    email: string;
    userName: string;
    firstName: string;
    middleName: string | null;
    lastName: string;
    dob: Date;
    password: string;
}

export interface AuthResponse{
    token: string;
}