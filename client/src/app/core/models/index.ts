export interface LoginDto { email: string; password: string; }
export interface RegisterDto { email: string; password: string; firstName: string; lastName: string; birthDate: string; }
export interface AuthResponse { token: string; expiresIn: number; }
export interface CurrentUser { id: string; email: string; name: string; roles: string[]; }

export interface EventDto { id: number; title: string; description: string; date: string; imagePath?: string; venueName: string; categoryNames: string[]; availableTickets: TicketSummaryDto[]; userId: string; capacity: number; currentRegistrations: number; remainingSeats: number;}
export interface CreateEventDto { title: string; description: string; date: string; imagePath?: string; venueId: number; categoryIds: number[]; }
export interface VenueDto { id: number; name: string; address: string; capacity: number; imagePath?: string; }
export interface CreateVenueDto { name: string; address: string; capacity: number; imagePath?: string; }
export interface CategoryDto { id: number; name: string; }
export interface CreateCategoryDto { name: string; }
export interface TicketSummaryDto { id: number; name: string; price: number; }
export interface TicketDto { id: number; typeName: string; price: number; eventId: number; eventTitle: string; }
export interface CreateTicketDto { typeName: string; price: number; eventId: number; }
export interface RegistrationDto { id: number; participantName: string; registrationDate: string; ticketId: number; ticketName: string; ticketPrice: number; eventTitle: string; eventDate: string; }
export interface CreateRegistrationDto { ticketId: number; participantName: string; }
export interface UserDto { id: string; userName: string; email: string; firstName: string; lastName: string; birthDate: string; }
export interface CreateUserDto { email: string; firstName: string; lastName: string; birthDate: string; }
export interface UpdateUserDto { email: string; firstName: string; lastName: string; birthDate: string; }
